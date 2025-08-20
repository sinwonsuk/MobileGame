using BackEnd;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class FoodLevelAutoSave : MonoBehaviour, IAutoSavable
{
	public static FoodLevelAutoSave Instance { get; private set; }
	private const string TABLE = "FOOD_LEVELS";

	[SerializeField] private FoodData[] allFoods;

	//private readonly Dictionary<string, int> levels = new();
	//private readonly HashSet<string> dirtyKeys = new();

	void Awake()
	{
		if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
		else { Destroy(gameObject); }
	}

	public IEnumerator InsertLevelIfNotExists(string ownerIndate)
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

			Backend.GameData.Get("FOOD_LEVELS", where, 100, offset, callback =>
			{
				bro = callback;
				isDone = true;
			});

			yield return new WaitUntil(() => isDone);

			if (!bro.IsSuccess())
			{
				Debug.LogError("[InsertFoodsIfNotExists] 조회 실패: " + bro.GetMessage());
				yield break;
			}

			var rows = bro.FlattenRows();
			foreach (var rowObj in rows)
			{
				var row = rowObj as LitJson.JsonData;
				if (row == null) continue;

				existingIndates.Add(row["foodIndate"].ToString());
			}


			var json = LitJson.JsonMapper.ToObject(bro.GetReturnValue());
			offset = json.ContainsKey("offset") ? json["offset"].ToString() : null;
			isEnd = string.IsNullOrEmpty(offset);
		}

		foreach (var emp in allFoods)
		{
			if (!existingIndates.Contains(emp.indate))
			{
				Param param = new Param();
				param.Add("foodIndate", emp.indate);
				param.Add("foodLevel", 1);

				bool done = false;
				BackendReturnObject insertBro = null;

				Backend.GameData.Insert("FOOD_LEVELS", param, callback =>
				{
					insertBro = callback;
					done = true;
				});

				yield return new WaitUntil(() => done);

				if (insertBro.IsSuccess())
				{
					Debug.Log($"[Food Level Insert 성공] {emp.indate}");
					emp.Level = 1;
					emp.isDirty = false;
				}
				else
				{
					Debug.LogError($"[Food Level Insert 실패] {emp.indate} : {insertBro.GetMessage()}");
				}
			}
		}
	}

	public IEnumerator LoadLevelData(string ownerIndate)
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
				Backend.GameData.Get("FOOD_LEVELS", where, 100, callback =>
				{
					bro = callback;
					isDone = true;
				});
			}
			else
			{
				Backend.GameData.Get("FOOD_LEVELS", where, 100, firstKey, callback =>
				{
					bro = callback;
					isDone = true;
				});
			}

			yield return new WaitUntil(() => isDone);

			if (!bro.IsSuccess())
			{
				Debug.LogError("[LoadFoodLevelData] 실패: " + bro.GetMessage());
				yield break;
			}

			var rows = bro.FlattenRows();
			foreach (var rowObj in rows)
			{
				var row = rowObj as LitJson.JsonData;
				if (row == null) continue;

				string empIndate = row["foodIndate"].ToString();
				int level = int.Parse(row["foodLevel"].ToString());

				var emp = allFoods.FirstOrDefault(e => e.indate == empIndate);
				if (emp != null)
				{
					emp.Level = level;
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
				Debug.LogWarning($"[LoadFoodLevelData] firstKey 파싱 실패 -> 종료 처리: {e.Message}");
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
			Debug.LogWarning("[AutoSave 차단] Food Level 데이터 로딩 안 됨");
			return;
		}

		SaveLevelData();
	}

	public void SaveLevelData()
	{
		string ownerIndate = Backend.UserInDate;

		foreach (var emp in allFoods)
		{
			if (!emp.isDirty) continue;

			Where where = new Where();
			where.Equal("owner_inDate", ownerIndate);
			where.Equal("foodIndate", emp.indate);

			Param param = new Param();
			param.Add("foodLevel", emp.Level);

			Backend.GameData.Update("FOOD_LEVELS", where, param, bro =>
			{
				if (bro.IsSuccess())
					emp.isDirty = false;
				//Debug.Log("음식 레벨 저장 완료 : " + bro);
				else
					Debug.LogError("게임 정보 수정 실패 : " + bro);
			});
			
		}

		Debug.Log("[FoodLevel] 변경된 Food Level 데이터 저장 완료");
	}

	//void OnApplicationQuit()
	//{
	//	SaveLevelData();
	//}
	private bool employeeDataLoaded = false;
}
