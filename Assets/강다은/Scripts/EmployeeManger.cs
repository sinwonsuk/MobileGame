using BackEnd;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EmployeeManager : MonoBehaviour, IAutoSavable
{
    public static EmployeeManager Instance { get; private set; }

    [Header("Config: 전체 직원 데이터")]
    public StaffStatsSO[] allEmployees;

    [Header("Config: 전체 런타임 직원 데이터")]
    public RuntimeStaffStatsSO[] allRunTimeEmployees;

    [Header("화살표 프리팹")]
    public GameObject arrowPrefab;

    [Header("슬롯 리스트")]
    public List<EmployeeSlot> slots = new List<EmployeeSlot>();

    // 위치 포인트를 동적으로 관리
    public List<Transform> dynamicPlacementPoints = new List<Transform>();

    // 현재 배치 중인 직원
    private EmployeeSlot currentPlacingSlot = null;
    // 생성된 화살표 목록
    private List<GameObject> activeArrows = new List<GameObject>();

    private bool employeeDataLoaded = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        InitializeDisplayNamesFromStatic();
        for (int i = 0; i < allEmployees.Length; i++)
        {
            slots.Add(new EmployeeSlot(allEmployees[i], allRunTimeEmployees[i]));
        }
    }
    private void Start()
    {
        LoadPlacementState();
    }

    public void RegisterPlacementPoint(Transform point)
    {
        if (!dynamicPlacementPoints.Contains(point))
            dynamicPlacementPoints.Add(point);
        Debug.Log($"{point.name} 등록됨, 현재 개수: {dynamicPlacementPoints.Count}");
    }

    // 위치 전체 초기화 (씬 전환 등)
    public void ClearPlacementPoints()
    {
        dynamicPlacementPoints.Clear();
    }

    // 직원 배치 시작
    public void StartPlacement(EmployeeSlot slot)
    {
        ClearArrows();
        currentPlacingSlot = slot;

        for (int i = 0; i < dynamicPlacementPoints.Count; i++)
        {
            var arrow = Instantiate(arrowPrefab, dynamicPlacementPoints[i].position, Quaternion.identity, dynamicPlacementPoints[i]);
            var arrowClicker = arrow.GetComponent<ArrowClicker>();
            if (arrowClicker != null)
                arrowClicker.SetIndex(i);
            activeArrows.Add(arrow);
        }
    }
    public void ReleaseEmployee(EmployeeSlot slot)
    {
        if (!slot.IsAssigned) return;
        int idx = slot.runtimeData.assignedIndex;
        if (idx >= 0 && idx < dynamicPlacementPoints.Count)
        {
            Transform point = dynamicPlacementPoints[idx];
            foreach (Transform child in point)
            {
                Destroy(child.gameObject);
            }
        }
        // 반드시 배치정보 리셋!
        slot.runtimeData.isAssigned = false;
        slot.runtimeData.assignedIndex = -1;
        slot.runtimeData.isDirty = true;

        SavePlacementState();
        FindAnyObjectByType<EmployeeInventoryUI>()?.RefreshUI();
    }




    // 직원 실제 배치
    public void PlaceEmployee(int index)
    {
        if (currentPlacingSlot == null) return;

        // 1. 기존에 이 위치에 배치된 직원 해제 (데이터만)
        foreach (var slot in slots)
        {
            if (slot.runtimeData.isAssigned && slot.runtimeData.assignedIndex == index)
            {
                slot.runtimeData.isAssigned = false;
                slot.runtimeData.assignedIndex = -1;
                slot.runtimeData.isDirty = true;
            }
        }

        //  2. 해당 위치에 있는 프리팹 모두 삭제
        Transform point = dynamicPlacementPoints[index];
        foreach (Transform child in point)
        {
            Destroy(child.gameObject);
        }

        //  3. 새로운 직원 프리팹 배치
        var staffPrefab = currentPlacingSlot.staffData.itemPrefab;
        if (staffPrefab == null)
        {
            Debug.LogError("직원 프리팹이 연결되지 않았음!");
            return;
        }
        GameObject staffObj = Instantiate(staffPrefab, point.position, Quaternion.identity, point);
        staffObj.name = staffPrefab.name;

        //  4. 새로운 직원의 데이터 업데이트
        currentPlacingSlot.runtimeData.isAssigned = true;
        currentPlacingSlot.runtimeData.assignedIndex = index;
        currentPlacingSlot.runtimeData.isDirty = true;

        SavePlacementState();
        ClearArrows();
        FindAnyObjectByType<EmployeeInventoryUI>()?.RefreshUI();
    }



    public void SavePlacementState()
    {
        foreach (var slot in slots)
        {
            PlayerPrefs.SetInt($"emp_{slot.staffData.indate}_assigned", slot.runtimeData.isAssigned ? 1 : 0);
            PlayerPrefs.SetInt($"emp_{slot.staffData.indate}_idx", slot.runtimeData.assignedIndex);
        }
        PlayerPrefs.Save();
    }
    public void LoadPlacementState()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            var slot = slots[i];
            bool isAssigned = PlayerPrefs.GetInt($"emp_{slot.staffData.indate}_assigned", 0) == 1;
            int idx = PlayerPrefs.GetInt($"emp_{slot.staffData.indate}_idx", -1);

            slot.runtimeData.isAssigned = isAssigned;
            slot.runtimeData.assignedIndex = idx;

            if (isAssigned && idx >= 0 && idx < dynamicPlacementPoints.Count)
            {
                Transform point = dynamicPlacementPoints[idx];
                // 기존 프리팹 모두 삭제(중복방지)
                foreach (Transform child in point)
                    Destroy(child.gameObject);

                // 새로 생성
                var staffPrefab = slot.staffData.itemPrefab;
                if (staffPrefab == null) continue;

                GameObject staffObj = Instantiate(staffPrefab, point.position, Quaternion.identity, point);
                staffObj.name = staffPrefab.name;
            }
        }
    }

    // 화살표 오브젝트 제거
    private void ClearArrows()
    {
        foreach (var go in activeArrows) Destroy(go);
        activeArrows.Clear();
        currentPlacingSlot = null;
    }

    // 씬 변경 시 포인트 초기화
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ClearPlacementPoints(); // 씬 바뀌면 초기화(중복 방지)
    }
    public IEnumerator InsertEmployeesIfNotExists(string ownerIndate)
    {
        string offset = "";
        bool isEnd = false;
        HashSet<string> existingIndates = new HashSet<string>();

        while (!isEnd)
        {
            bool isDone = false;
            BackendReturnObject bro = null;

            var where = new Where();
            where.Equal("owner_inDate", ownerIndate);

            Backend.GameData.Get("EMPLOYEE_PLAYER", where, 100, offset, callback =>
            {
                bro = callback;
                isDone = true;
            });

            yield return new WaitUntil(() => isDone);

            if (!bro.IsSuccess())
            {
                Debug.LogError("[InsertEmployeesIfNotExists] 조회 실패: " + bro.GetMessage());
                yield break;
            }

            var rows = bro.FlattenRows();
            foreach (var rowObj in rows)
            {
                var row = rowObj as LitJson.JsonData;
                if (row == null) continue;

                existingIndates.Add(row["employeeIndate"].ToString());
            }


            var json = LitJson.JsonMapper.ToObject(bro.GetReturnValue());
            offset = json.ContainsKey("offset") ? json["offset"].ToString() : null;
            isEnd = string.IsNullOrEmpty(offset);
        }

        foreach (var emp in allRunTimeEmployees)
        {
            if (!existingIndates.Contains(emp.indate))
            {
                Param param = new Param();
                param.Add("employeeIndate", emp.indate);
                param.Add("employeeCustomLevel", "0");
                param.Add("employeeName", emp.displayName);

                bool done = false;
                BackendReturnObject insertBro = null;

                Backend.GameData.Insert("EMPLOYEE_PLAYER", param, callback =>
                {
                    insertBro = callback;
                    done = true;
                });

                yield return new WaitUntil(() => done);

                if (insertBro.IsSuccess())
                {
                    Debug.Log($"[직원 Insert 성공] {emp.indate}");
                    emp.level = 0;
                    emp.isDirty = false;
                }
                else
                {
                    Debug.LogError($"[직원 Insert 실패] {emp.indate} : {insertBro.GetMessage()}");
                }
            }
        }
    }

    public IEnumerator LoadEmployeeData(string ownerIndate)
    {
        string firstKey = null;
        bool isEnd = false;

        while (!isEnd)
        {
            bool isDone = false;
            BackendReturnObject bro = null;

            var where = new Where();
            where.Equal("owner_inDate", ownerIndate);

            if (string.IsNullOrEmpty(firstKey))
            {
                Backend.GameData.Get("EMPLOYEE_PLAYER", where, 100, callback =>
                {
                    bro = callback;
                    isDone = true;
                });
            }
            else
            {
                Backend.GameData.Get("EMPLOYEE_PLAYER", where, 100, firstKey, callback =>
                {
                    bro = callback;
                    isDone = true;
                });
            }

            yield return new WaitUntil(() => isDone);

            if (!bro.IsSuccess())
            {
                Debug.LogError("[LoadEmployeeData] 실패: " + bro.GetMessage());
                yield break;
            }

            var rows = bro.FlattenRows();
            foreach (var rowObj in rows)
            {
                var row = rowObj as LitJson.JsonData;
                if (row == null) continue;

                string empIndate = row["employeeIndate"].ToString();
                int level = int.Parse(row["employeeCustomLevel"].ToString());

                var emp = allRunTimeEmployees.FirstOrDefault(e => e.indate == empIndate);
                if (emp != null)
                {
                    emp.level = level;
                    emp.isDirty = false;
                }
            }

            try
            {
                var json = LitJson.JsonMapper.ToObject(bro.GetReturnValue());
                if (json.ContainsKey("firstKey") && json["firstKey"] != null)
                {
                    firstKey = json["firstKey"]["inDate"]["S"].ToString();
                }
                else
                {
                    isEnd = true;
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[LoadEmployeeData] firstKey 파싱 실패 -> 종료 처리: {e.Message}");
                isEnd = true;
            }
        }

        employeeDataLoaded = true;
        AutoSaveManager.Instance?.RegisterAutoSavable(this);
    }


    public void AutoSave()
    {
        if (!employeeDataLoaded)
        {
            Debug.LogWarning("[AutoSave 차단] 직원 데이터 로딩 안 됨");
            return;
        }

        SaveEmployeeData();
    }

    public void SaveEmployeeData()
    {
        string ownerIndate = Backend.UserInDate;

        foreach (var emp in allRunTimeEmployees)
        {
            if (!emp.isDirty) continue;

            Where where = new Where();
            where.Equal("owner_inDate", ownerIndate);
            where.Equal("employeeIndate", emp.indate);

            Param param = new Param();
            param.Add("employeeCustomLevel", emp.level);
            param.Add("employeeName", emp.displayName);

            Backend.GameData.Update("EMPLOYEE_PLAYER", where, param);
            emp.isDirty = false;
        }

        Debug.Log("[EmployeeManager] 변경된 직원 데이터 저장 완료");
    }

    private void InitializeDisplayNamesFromStatic()
    {
        foreach (var runtime in allRunTimeEmployees)
        {
            var staticData = allEmployees.FirstOrDefault(s => s.indate == runtime.indate);
            if (staticData != null)
            {
                runtime.displayName = staticData.displayName;
            }
            else
            {
                Debug.LogWarning($"[초기화 실패] {runtime.indate} 에 해당하는 마스터 직원 데이터가 없습니다.");
            }
        }
    }

}

