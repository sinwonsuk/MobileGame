using BackEnd;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InteriorManager : MonoBehaviour, IAutoSavable
{
    public static InteriorManager Instance { get; private set; }

    [Header("Config: 모든 인테리어 데이터")]
    public InteriorData[] allInteriors;

    [Header("Runtime: 인테리어 상태 데이터 (SO가 진실)")]
    public RunTimeInteriorData[] allRunTimeInteriors;

    public List<InteriorSlot> slots = new List<InteriorSlot>();
    public event Action OnInteriorChanged;

    [SerializeField] private string targetSceneName = "SampleScene";

    private bool FurnitureDataLoaded = false;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        DontDestroyOnLoad(gameObject);

        // 슬롯 초기화
        slots.Clear();
        for (int i = 0; i < allInteriors.Length; i++)
        {
            slots.Add(new InteriorSlot(allInteriors[i], allRunTimeInteriors[i]));
            allRunTimeInteriors[i].instance = null;
            if (allInteriors[i].alwaysInstalled) // ★ 고정이면 무조건 소유/설치
            {
                allRunTimeInteriors[i].isOwned = true;
                allRunTimeInteriors[i].isUsed = true;
                allRunTimeInteriors[i].isDirty = true;
            }
        }

    }
    
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        // 게임 시작 시 현재 씬이 타겟인지 확인하여 처리
        if (ShouldSpawnInCurrentScene())
            RefreshInstalledInteriors();
        else
            DespawnAllInstances();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (ShouldSpawnInCurrentScene())
            RefreshInstalledInteriors();
        else
            DespawnAllInstances();
    }

    private bool ShouldSpawnInCurrentScene()
    {
        if (string.IsNullOrEmpty(targetSceneName)) return true; // 타겟 미지정이면 모든 씬 허용
        return SceneManager.GetActiveScene().name == targetSceneName;
    }
    private void DespawnAllInstances()
    {
        foreach (var slot in slots)
        {
            if (slot.runtimeData.instance != null)
            {
                Destroy(slot.runtimeData.instance);
                slot.runtimeData.instance = null;
            }
        }

    }

    public void AcquireInterior(string name)
    {
        var slot = slots.Find(s => s.data.interiorName == name);
        if (slot == null) return;

        slot.runtimeData.isOwned = true;
        slot.runtimeData.isDirty = true;
        OnInteriorChanged?.Invoke();
    }

    public void UseInterior(string name)
    {
        var slot = slots.Find(s => s.data.interiorName == name);
        if (slot == null || !slot.runtimeData.isOwned) return;

        // ★ 고정 설치면 해제 금지 + 상태 보정 후 종료
        if (slot.data.alwaysInstalled)
        {
            slot.runtimeData.isOwned = true;
            slot.runtimeData.isUsed = true;
            slot.runtimeData.isDirty = true;

            // 현재 씬에서만 스폰 유지
            if (ShouldSpawnInCurrentScene())
            {
                if (slot.runtimeData.instance == null)
                {
                    var go = Instantiate(slot.data.prefab, slot.data.placementPosition, Quaternion.identity);
                    slot.runtimeData.instance = go;
                }
            }
            else
            {
                // 타겟 씬이 아니면 인스턴스 제거
                if (slot.runtimeData.instance != null)
                {
                    Destroy(slot.runtimeData.instance);
                    slot.runtimeData.instance = null;
                }
            }

            OnInteriorChanged?.Invoke();
            return; // ★ 토글 금지
        }

        // ===== 이하 기존 토글 로직 유지 =====
        bool toUse = !(slot.runtimeData.isUsed);
        slot.runtimeData.isUsed = toUse;
        slot.runtimeData.isDirty = true;

        if (!ShouldSpawnInCurrentScene())
        {
            if (slot.runtimeData.instance != null)
            {
                Destroy(slot.runtimeData.instance);
                slot.runtimeData.instance = null;
            }
            OnInteriorChanged?.Invoke();
            return;
        }

        if (toUse)
        {
            if (slot.runtimeData.instance == null)
            {
                Vector3 pos = slot.data.placementPosition;
                var go = Instantiate(slot.data.prefab, pos, Quaternion.identity);
                slot.runtimeData.instance = go;
            }
        }
        else
        {
            if (slot.runtimeData.instance != null)
            {
                Destroy(slot.runtimeData.instance);
                slot.runtimeData.instance = null;
            }
        }

        OnInteriorChanged?.Invoke();
    }

    // InteriorManager.cs
    private void RefreshInstalledInteriors()
    {
        int spawnCount = 0;

        foreach (var slot in slots)
        {
            // ★ 항상 설치 항목은 isOwned/isUsed 보정
            if (slot.data.alwaysInstalled)
            {
                slot.runtimeData.isOwned = true;
                slot.runtimeData.isUsed = true;
            }

            // 기존 인스턴스 정리
            if (slot.runtimeData.instance != null)
            {
                Destroy(slot.runtimeData.instance);
                slot.runtimeData.instance = null;
            }

            // ★ 현재 씬에서만 스폰(ShouldSpawnInCurrentScene가 true일 때)
            if (slot.runtimeData.isOwned && slot.runtimeData.isUsed && ShouldSpawnInCurrentScene())
            {
                var go = Instantiate(slot.data.prefab, slot.data.placementPosition, Quaternion.identity);
                slot.runtimeData.instance = go;
                spawnCount++;
            }
        }

        OnInteriorChanged?.Invoke();
    }



    public IEnumerator InsertFurnitureIfNotExists(string ownerIndate)
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

            Backend.GameData.Get("FURNITURE_PLAYER", where, 100, offset, callback =>
            {
                bro = callback;
                isDone = true;
            });

            yield return new WaitUntil(() => isDone);

            if (!bro.IsSuccess())
            {
                Debug.LogError("[InsertFurnitureIfNotExists] 조회 실패: " + bro.GetMessage());
                yield break;
            }

            var rows = bro.FlattenRows();
            foreach (var rowObj in rows)
            {
                var row = rowObj as LitJson.JsonData;
                if (row == null) continue;

                existingIndates.Add(row["furnitureIndate"].ToString());
            }

            var json = LitJson.JsonMapper.ToObject(bro.GetReturnValue());
            offset = json.ContainsKey("offset") ? json["offset"].ToString() : null;
            isEnd = string.IsNullOrEmpty(offset);
        }

        foreach (var emp in allRunTimeInteriors)
        {
            if (!existingIndates.Contains(emp.indate))
            {
                Param param = new Param();
                param.Add("furnitureIndate", emp.indate);
                param.Add("interiorName", emp.interiorName);
                param.Add("isOwned", false);
                param.Add("isUsed", false);

                bool done = false;
                BackendReturnObject insertBro = null;

                Backend.GameData.Insert("FURNITURE_PLAYER", param, callback =>
                {
                    insertBro = callback;
                    done = true;
                });

                yield return new WaitUntil(() => done);

                if (insertBro.IsSuccess())
                {
                    //Debug.Log($"[가구 Insert 성공] {emp.indate}");
                    emp.isDirty = true;
                }
                else
                {
                    Debug.LogError($"[가구 Insert 실패] {emp.indate} : {insertBro.GetMessage()}");
                }
            }
        }
    }

    public IEnumerator LoadFurnitureData(string ownerIndate)
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
                Backend.GameData.Get("FURNITURE_PLAYER", where, 100, callback =>
                {
                    bro = callback;
                    isDone = true;
                });
            }
            else
            {
                Backend.GameData.Get("FURNITURE_PLAYER", where, 100, firstKey, callback =>
                {
                    bro = callback;
                    isDone = true;
                });
            }

            yield return new WaitUntil(() => isDone);

            if (!bro.IsSuccess())
            {
                Debug.LogError("[LoadFurnitureData] 실패: " + bro.GetMessage());
                yield break;
            }

            var rows = bro.FlattenRows();
            foreach (var rowObj in rows)
            {
                var row = rowObj as LitJson.JsonData;
                if (row == null) continue;

                string empIndate = row["furnitureIndate"].ToString();
                bool isOwned = bool.Parse(row["isOwned"].ToString());
                bool isUsed = bool.Parse(row["isUsed"].ToString());

                var emp = allRunTimeInteriors.FirstOrDefault(e => e.indate == empIndate);
                if (emp != null)
                {
                    emp.isDirty = false;
                    emp.isOwned = isOwned;
                    emp.isUsed = isUsed;
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
                Debug.LogWarning($"[LoadFurnitureData] firstKey 파싱 실패 -> 종료 처리: {e.Message}");
                isEnd = true;
            }
        }

        FurnitureDataLoaded = true;
        AutoSaveManager.Instance?.RegisterAutoSavable(this);

        if (ShouldSpawnInCurrentScene())
            RefreshInstalledInteriors();
        else
            DespawnAllInstances();
    }

    public void AutoSave()
    {
        if (!FurnitureDataLoaded)
        {
            Debug.LogWarning("[AutoSave 차단] 가구 데이터 로딩 안 됨");
            return;
        }

        SaveFurnitureData();
    }

    public void SaveFurnitureData()
    {
        string ownerIndate = Backend.UserInDate;

        foreach (var emp in allRunTimeInteriors)
        {
            if (!emp.isDirty) continue;

            Where where = new Where();
            where.Equal("owner_inDate", ownerIndate);
            where.Equal("furnitureIndate", emp.indate);

            Param param = new Param();
            param.Add("isOwned", emp.isOwned);
            param.Add("isUsed", emp.isUsed);

            Backend.GameData.Update("FURNITURE_PLAYER", where, param, bro =>
            {
                if (bro.IsSuccess()) ;
                //Debug.Log("가구 저장 완료 : " + bro);
                else
                    Debug.LogError("게임 정보 수정 실패 : " + bro);
            });
            emp.isDirty = false;
        }

        //Debug.Log("[FurnitureManager] 변경된 가구 데이터 저장 완료");
    }
}
