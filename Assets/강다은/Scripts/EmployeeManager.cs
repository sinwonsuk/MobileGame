using BackEnd;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EmployeeManager : MonoBehaviour, IAutoSavable
{
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
        //InitializeDisplayNamesFromStatic();
        for (int i = 0; i < allEmployees.Length; i++)
        {
            slots.Add(new EmployeeSlot(allEmployees[i], allRunTimeEmployees[i]));
        }
        for (int i = 0; i < allRunTimeEmployees.Length; i++)
        {
            var run = allRunTimeEmployees[i];
            var stat = allEmployees[i];
            run.RecalcWith(stat);
        }
        NotifyStaffChanged();
    }

    private void FinalizePlacementPoints()
    {
        // 형제 순서(=Hierarchy 순서)로 정렬
        dynamicPlacementPoints = dynamicPlacementPoints
            .OrderBy(t => t.GetSiblingIndex())
            .ToList();

        Debug.Log("[Placement 정렬] " +
            string.Join(", ", dynamicPlacementPoints.Select(t => $"{t.GetSiblingIndex()}:{t.name}")));
    }
    public void RegisterPlacementPoint(Transform point)
    {
        if (!dynamicPlacementPoints.Contains(point))
            dynamicPlacementPoints.Add(point);

        _needsSort = true; // 정렬 예약
        Debug.Log($"{point.name} 등록됨, 현재 개수: {dynamicPlacementPoints.Count}");
    }


    // 위치 전체 초기화
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
				Destroy(child.gameObject);
		}

		// 반드시 배치정보 리셋
		slot.runtimeData.isAssigned = false;
		slot.runtimeData.assignedIndex = -1;
		slot.runtimeData.isDirty = true;

		FindAnyObjectByType<EmployeeInventoryUI>()?.RefreshUI();

		AutoSaveManager.Instance?.ForceFlushSoon(0.25f);
	}

    // 직원 실제 배치
    // EmployeeManager.cs 안에 넣어 사용

    public void PlaceEmployee(int index)
    {
        if (currentPlacingSlot == null)
            return;

        // 현재 배치하려는 슬롯의 직원 타입
        var staffType = currentPlacingSlot.staffData.staffType; // StaffType.hunter / StaffType.restaurant

        // 유효 범위 체크 (전투=0~1, 경영=2~3)
        bool invalidForHunter = (staffType == StaffType.hunter) && !(index >= 0 && index <= 1);
        bool invalidForRestaurant = (staffType == StaffType.restaurant) && !(index >= 2 && index <= 3);

        if (invalidForHunter || invalidForRestaurant)
        {
            // 메시지 결정
            string msg;
            if (staffType == StaffType.hunter)
                msg = "경영직원이 아닙니다";
            else if (staffType == StaffType.restaurant)
                msg = "전투직원이 아닙니다";
            else
                msg = "잘못된 위치입니다";

            // PopupManager로 교체
            PopupManager.Show(msg, () =>
            {
                ClearArrows(); // 확인 누르면 화살표 제거
            });
            return;
        }

        // ---------- 여기서부터는 정상 배치 로직 ----------

        // 같은 인덱스(=같은 자리)에 이미 누가 배치되어 있으면 데이터만 해제
        foreach (var slot in slots)
        {
            if (slot.runtimeData.isAssigned && slot.runtimeData.assignedIndex == index)
            {
                slot.runtimeData.isAssigned = false;
                slot.runtimeData.assignedIndex = -1;
                slot.runtimeData.isDirty = true;
            }
        }

        // 해당 포인트의 기존 프리팹 제거
        Transform point = dynamicPlacementPoints[index];
        if (point != null)
        {
            // 자식 전부 제거
            for (int i = point.childCount - 1; i >= 0; i--)
                Destroy(point.GetChild(i).gameObject);
        }

        // 새 프리팹 스폰
        var staffPrefab = currentPlacingSlot.staffData.itemPrefab;
        if (staffPrefab == null)
        {
            Debug.LogError("[PlaceEmployee] staff prefab이 비어있습니다.");
            ClearArrows();
            return;
        }

        GameObject staffObj = Instantiate(staffPrefab, point.position, Quaternion.identity, point);
        staffObj.name = staffPrefab.name;

        // Init 호출 (Stats 연결)
        var staffBase = staffObj.GetComponent<StaffBase>();
        if (staffBase != null)
            staffBase.Init(currentPlacingSlot.staffData, currentPlacingSlot.runtimeData);

        // 데이터 업데이트
        currentPlacingSlot.runtimeData.isAssigned = true;
        currentPlacingSlot.runtimeData.assignedIndex = index;
        currentPlacingSlot.runtimeData.isDirty = true;

        // 화살표 클리어 & UI 갱신
        ClearArrows();
        FindAnyObjectByType<EmployeeInventoryUI>()?.RefreshUI();

		AutoSaveManager.Instance?.ForceFlushSoon(0.25f);
	}

    // 외부에서도 부를 수 있게 public
    public void ClearArrows()
    {
        if (activeArrows != null)
        {
            foreach (var go in activeArrows)
                if (go != null) Destroy(go);
            activeArrows.Clear();
        }
        currentPlacingSlot = null;
    }


    // 특정 인덱스 위치에 즉시 배치 (DB 더티까지 처리)
    public bool TryPlaceAtIndex(RuntimeStaffStatsSO run, StaffStatsSO stat, int index)
    {
        if (stat == null || run == null) return false;
        if (dynamicPlacementPoints == null || index < 0 || index >= dynamicPlacementPoints.Count)
        {
            Debug.LogWarning($"[TryPlaceAtIndex] 잘못된 index: {index}");
            return false;
        }

        // 같은 위치에 이미 배치된 직원이 있으면 데이터만 해제
        foreach (var s in slots)
        {
            if (s.runtimeData.isAssigned && s.runtimeData.assignedIndex == index)
            {
                s.runtimeData.isAssigned = false;
                s.runtimeData.assignedIndex = -1;
                s.runtimeData.isDirty = true;
            }
        }

        // 해당 포인트의 기존 프리팹 제거
        Transform point = dynamicPlacementPoints[index];
        for (int c = point.childCount - 1; c >= 0; c--)
            Destroy(point.GetChild(c).gameObject);

        // 새 프리팹 배치
        var prefab = stat.itemPrefab;
        if (prefab == null)
        {
            Debug.LogError("[TryPlaceAtIndex] prefab 없음");
            return false;
        }
        var go = Instantiate(prefab, point.position, Quaternion.identity, point);
        go.name = prefab.name;

        // Init
        var baseComp = go.GetComponent<StaffBase>();
        if (baseComp != null) baseComp.Init(stat, run);

        // 런타임 데이터 갱신(+저장 플래그)
        run.isOwned = true;
        run.isAssigned = true;
        run.assignedIndex = index;
        run.isDirty = true;

        NotifyStaffChanged();
		AutoSaveManager.Instance?.ForceFlushSoon(0.25f);
		return true;
    }

    // 슬롯 버전(있으면 편의상 사용)
    public bool TryPlaceAtIndex(EmployeeSlot slot, int index)
    {
        if (slot == null) return false;
        return TryPlaceAtIndex(slot.runtimeData, slot.staffData, index);
    }


    public void LoadPlacementState()
    {
        if (dynamicPlacementPoints.Count == 0)
        {
            Debug.LogWarning("[LoadPlacementState] points=0 → skip");
            return;
        }

        // 0) 먼저, 배치 안 된 슬롯의 stale index 정리
        for (int i = 0; i < slots.Count; i++)
        {
            var s = slots[i].runtimeData;
            if (!s.isAssigned && s.assignedIndex != -1)
                s.assignedIndex = -1;
        }

        for (int i = 0; i < slots.Count; i++)
        {
            var slot = slots[i];
            bool isAssigned = slot.runtimeData.isAssigned;
            int idx = slot.runtimeData.assignedIndex;
            bool owned = slot.runtimeData.isOwned;
            int lvl = slot.runtimeData.level;

            if (isAssigned && idx >= 0 && idx < dynamicPlacementPoints.Count)
            {
                Transform point = dynamicPlacementPoints[idx];
                for (int c = point.childCount - 1; c >= 0; c--)
                    Destroy(point.GetChild(c).gameObject);
            }

            bool canSpawn = isAssigned
                            && idx >= 0 && idx < dynamicPlacementPoints.Count
                            && (owned || lvl > 0);

            if (canSpawn)
            {
                var prefab = slot.staffData.itemPrefab;
                if (prefab == null)
                {
                    Debug.LogWarning($"[LOAD-SKIP] {slot.runtimeData.displayName} idx={idx} → prefab=null");
                    continue;
                }

                Transform point = dynamicPlacementPoints[idx];
                var go = Instantiate(prefab, point.position, Quaternion.identity, point);
                go.name = prefab.name;

                var baseComp = go.GetComponent<StaffBase>();
                if (baseComp != null)
                    baseComp.Init(slot.staffData, slot.runtimeData);

                Debug.Log($"[LOAD-SPAWN] {slot.runtimeData.displayName} idx={idx}");
            }
            else
            {
                // 정말 무효(미소유 & 레벨0)면만 상태 초기화
                if (!(owned || lvl > 0))
                {
                    slot.runtimeData.isAssigned = false;
                    slot.runtimeData.assignedIndex = -1;
                }

            }
        }
    }

	//private void OnApplicationQuit()
	//{
	//	SaveEmployeeData();
	//}


	private IEnumerator DelayedPlacementRestore()
    {
        // 모든 PlacementPointRegister.Start()가 끝나길 대기
        yield return null;
        yield return new WaitForEndOfFrame();

        if (_needsSort)
        {
            FinalizePlacementPoints();
            _needsSort = false;
        }

        LoadPlacementState();
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
        if (scene.name == "SampleScene")
        {
            StartCoroutine(DelayedPlacementRestore());
        }
    }

    //다은
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
                param.Add("employeeCustomLevel", 0);
                param.Add("employeeName", emp.displayName);
                param.Add("isOwned", false);
                param.Add("isAssigned", false);
                param.Add("assignedIndex", -1);

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
                //string displayName = row["employeeName"].ToString();
                bool isOwned = bool.Parse(row["isOwned"].ToString());
                bool isAssigned = bool.Parse(row["isAssigned"].ToString());
                int assignedIndex = int.Parse(row["assignedIndex"].ToString());

				var emp = allRunTimeEmployees.FirstOrDefault(e => e.indate == empIndate);
                if (emp != null)
                {
                    emp.level = level;
                    emp.isDirty = false;
                    //emp.displayName = displayName;
                    emp.isOwned = isOwned;
                    emp.isAssigned = isAssigned;
                    emp.assignedIndex = assignedIndex;
                    var stat = allEmployees.FirstOrDefault(s => s.indate == emp.indate);
                    if (stat != null)
                    {
                        emp.RecalcWith(stat);   // 로드된 레벨값 기반으로 능력치 재계산
                    }
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
            param.Add("isOwned", emp.isOwned);
            param.Add("isAssigned", emp.isAssigned);
            param.Add("assignedIndex", emp.assignedIndex);

			Backend.GameData.Update("EMPLOYEE_PLAYER", where, param, bro =>
            {
				if (bro.IsSuccess())
					emp.isDirty = false;
				else
					Debug.LogError("게임 정보 수정 실패 : " + bro);
			});
            
        }

        Debug.Log("[EmployeeManager] 변경된 직원 데이터 저장 완료");
    }

   //private void InitializeDisplayNamesFromStatic()
   //{
   //    foreach (var runtime in allRunTimeEmployees)
   //    {
   //        var staticData = allEmployees.FirstOrDefault(s => s.indate == runtime.indate);
   //        if (staticData != null)
   //        {
   //            runtime.displayName = staticData.displayName;
   //        }
   //        else
   //        {
   //            Debug.LogWarning($"[초기화 실패] {runtime.indate} 에 해당하는 마스터 직원 데이터가 없습니다.");
   //        }
   //    }
   //}


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
	public event Action OnStaffChanged;
	public void NotifyStaffChanged() => OnStaffChanged?.Invoke();
    private bool _needsSort = false;
}

