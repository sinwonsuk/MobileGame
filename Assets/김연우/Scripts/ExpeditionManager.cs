using BackEnd;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ExpeditionManager : MonoBehaviour, IAutoSavable
{
    public static ExpeditionManager Instance { get; private set; }

    [Header("정적 파견지 SO들 (예: 10개)")]
    public ExpeditionSO[] allExpeditions;

    [Header("런타임 파견지 SO들 (예: 10개)")]
    public RuntimeExpeditionSO[] allRuntimeExpeditions;

    private readonly Dictionary<string, (ExpeditionSO stat, RuntimeExpeditionSO run)> pairs = new();

    public event Action<string> OnChanged; // 특정 id 변경 알림

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        BuildPairs();
    }

    private void OnValidate()
    {
        BuildPairs();
    }

    private void BuildPairs()
    {
        pairs.Clear();
        if (allExpeditions == null || allRuntimeExpeditions == null) return;

        foreach (var run in allRuntimeExpeditions)
        {
            if (run == null) continue;
            run.SyncIdFromStatic();
        }
        var statById = allExpeditions
            ?.Where(s => s != null && !string.IsNullOrEmpty(s.Indate))
            .GroupBy(s => s.Indate).ToDictionary(g => g.Key, g => g.First());

        var runById = allRuntimeExpeditions
            ?.Where(r => r != null && !string.IsNullOrEmpty(r.indate))
            .GroupBy(r => r.indate).ToDictionary(g => g.Key, g => g.First());

        if (statById == null || runById == null) return;

        foreach (var id in statById.Keys)
        {
            if (!runById.ContainsKey(id))
            {
                Debug.LogWarning($"[ExpeditionManager] 런타임 SO 없음: {id}");
                continue;
            }
            pairs[id] = (statById[id], runById[id]);
        }

        foreach (var id in runById.Keys)
            if (!statById.ContainsKey(id))
                Debug.LogWarning($"[ExpeditionManager] 정적 SO 없음: {id}");
    }


    public bool CanStart(string id)
    {
        if (!pairs.TryGetValue(id, out var p)) return false;
        return !p.run.isRunning;
    }


    public bool StartExpedition(string id)
    {
        if (!pairs.TryGetValue(id, out var p)) return false;  
        if (p.run.isRunning) return false;

        p.run.StartNowUtc(p.stat.durationHours);
        p.run.isDirty = true;
        OnChanged?.Invoke(id);
		return true;
    }


    public TimeSpan GetRemaining(string id)
    {
        if (!pairs.TryGetValue(id, out var p)) return TimeSpan.Zero;
        if (!p.run.isRunning) return TimeSpan.Zero;

        var rem = p.run.ArriveUtc - DateTime.UtcNow;
        return rem > TimeSpan.Zero ? rem : TimeSpan.Zero;
    }

    public bool IsDone(string id)
    {
        if (!pairs.TryGetValue(id, out var p)) return false;

        if (p.run.isRunning && DateTime.UtcNow >= p.run.ArriveUtc)
        {
            p.run.isRunning = false;

            TryClaimReward(id);

            return true; 
        }
        return !p.run.isRunning && !string.IsNullOrEmpty(p.run.arriveUtcIso);
    }



    public bool TryClaimReward(string id)
    {
        if (!pairs.TryGetValue(id, out var p)) return false;

        if (p.run.isRunning || DateTime.UtcNow < p.run.ArriveUtc)
            return false;

        if (p.stat.rewards != null)
        {
            foreach (var r in p.stat.rewards)
            {
                if (r?.ingredientData == null) continue;
                InventoryManager.Instance.AddItem(r.ingredientData.indate, Mathf.Max(0, r.amount));
            }
        }

        p.run.Clear();

        OnChanged?.Invoke(id);
        return true;
    }


    public float GetProgress01(string id)
    {
        if (!pairs.TryGetValue(id, out var p)) return 0f;
        if (!p.run.isRunning || p.stat.durationHours <= 0f) return 0f;

        double total = TimeSpan.FromHours(p.stat.durationHours).TotalSeconds;
        double elapsed = Math.Clamp((DateTime.UtcNow - p.run.DepartUtc).TotalSeconds, 0, total);
        return (float)(elapsed / total);
    }

    public void ResetAll()
    {
        foreach (var kv in pairs) kv.Value.run.Clear();
    }

	public IEnumerator InsertExpeditionIfNotExists(string ownerIndate)
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

			Backend.GameData.Get("EXPEDITIONS_PLAYER", where, 100, offset, callback =>
			{
				bro = callback;
				isDone = true;
			});

			yield return new WaitUntil(() => isDone);

			if (!bro.IsSuccess())
			{
				Debug.LogError("[InsertExpeditionIfNotExists] 조회 실패: " + bro.GetMessage());
				yield break;
			}

			var rows = bro.FlattenRows();
			foreach (var rowObj in rows)
			{
				var row = rowObj as LitJson.JsonData;
				if (row == null) continue;

				existingIndates.Add(row["expenditionIndate"].ToString());
			}


			var json = LitJson.JsonMapper.ToObject(bro.GetReturnValue());
			offset = json.ContainsKey("offset") ? json["offset"].ToString() : null;
			isEnd = string.IsNullOrEmpty(offset);
		}

		foreach (var emp in allRuntimeExpeditions)
		{
			if (!existingIndates.Contains(emp.indate))
			{
				Param param = new Param();
				param.Add("expenditionIndate", emp.indate);
				param.Add("isRunning", false);
				param.Add("departUtcIso", "default");
				param.Add("arriveUtcIso", "default");

				bool done = false;
				BackendReturnObject insertBro = null;

				Backend.GameData.Insert("EXPEDITIONS_PLAYER", param, callback =>
				{
					insertBro = callback;
					done = true;
				});

				yield return new WaitUntil(() => done);

				if (insertBro.IsSuccess())
				{
					Debug.Log($"[파견 Insert 성공] {emp.indate}");
				}
				else
				{
					Debug.LogError($"[파견 Insert 실패] {emp.indate} : {insertBro.GetMessage()}");
				}
			}
		}
	}


	//다은
	public IEnumerator LoadExpeditionData(string ownerIndate)
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
				Backend.GameData.Get("EXPEDITIONS_PLAYER", where, 100, callback =>
				{
					bro = callback;
					isDone = true;
				});
			}
			else
			{
				Backend.GameData.Get("EXPEDITIONS_PLAYER", where, 100, firstKey, callback =>
				{
					bro = callback;
					isDone = true;
				});
			}

			yield return new WaitUntil(() => isDone);

			if (!bro.IsSuccess())
			{
				Debug.LogError("[LoadExeditionsData] 실패: " + bro.GetMessage());
				yield break;
			}

			var rows = bro.FlattenRows();
			foreach (var rowObj in rows)
			{
				var row = rowObj as LitJson.JsonData;
				if (row == null) continue;

				string empIndate = row["expenditionIndate"].ToString();
				bool isRunning = row["isRunning"].ToString() == "True";
				string departUtcIso = row["departUtcIso"].ToString();
				string arriveUtcIso = row["arriveUtcIso"].ToString();

				var emp = allRuntimeExpeditions.FirstOrDefault(e => e.indate == empIndate);
				if (emp != null)
				{
					emp.indate = empIndate;
					emp.isRunning = isRunning;
					emp.departUtcIso = departUtcIso;
					emp.arriveUtcIso = arriveUtcIso;
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
			Debug.LogWarning("[AutoSave 차단] 파견 데이터 로딩 안 됨");
			return;
		}

		SaveExeditionData();
	}

	public void SaveExeditionData()
	{
		string ownerIndate = Backend.UserInDate;

		foreach (var emp in allRuntimeExpeditions)
		{
			if (!emp.isDirty) continue;

			Where where = new Where();
			where.Equal("owner_inDate", ownerIndate);
			where.Equal("expenditionIndate", emp.indate);

			Param param = new Param();
			param.Add("isRunning", emp.isRunning);
			param.Add("departUtcIso", emp.departUtcIso);
			param.Add("arriveUtcIso", emp.arriveUtcIso);

			Backend.GameData.Update("EXPEDITIONS_PLAYER", where, param, bro =>
			{
				if (bro.IsSuccess())
					Debug.Log("파견 저장 완료 : " + bro);
				else
					Debug.LogError("게임 정보 수정 실패 : " + bro);
			});
			emp.isDirty = false;
		}

		Debug.Log("[ExpeditionManager] 변경된 파견 데이터 저장 완료");
	}

	private bool employeeDataLoaded = false;
	public IEnumerable<RuntimeExpeditionSO> EnumerateRuntime() => pairs.Values.Select(v => v.run);
}
