using BackEnd;
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class TutorialInit : MonoBehaviour, IAutoSavable
{
	public static TutorialInit Instance { get; private set; }
	public void AutoSave()
	{
		//Backend.GameInfo.Save("Tutorial", "true");
		SaveTuto();
	}

	public event Action OnInventoryChanged;
	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}
	public IEnumerator InsertTutoDataIfNotExists(string ownerIndate)
	{
		string firstKey = null;
		const int limit = 100;
		bool isEnd = false;

		HashSet<string> existingIndates = new HashSet<string>();

		while (!isEnd)
		{
			bool isDone = false;
			BackendReturnObject bro = null;

			var where = new Where();
			where.Equal("owner_inDate", ownerIndate);

			if (string.IsNullOrEmpty(firstKey))
			{
				Backend.GameData.Get("Tutorial", where, limit, callback =>
				{
					bro = callback;
					isDone = true;
				});
			}
			else
			{
				Backend.GameData.Get("Tutorial", where, limit, firstKey, callback =>
				{
					bro = callback;
					isDone = true;
				});
			}

			yield return new WaitUntil(() => isDone);

			if (bro == null || !bro.IsSuccess())
			{
				Debug.LogError($"[ERROR] Tutorial 서버 조회 실패: {bro}");
				yield break;
			}

			var rows = bro.FlattenRows();

			if (rows == null || rows.Count == 0)
			{
				Debug.LogWarning("[InsertTutoDataIfNotExists] 서버에서 받은 튜토 데이터 없음!");
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
				Debug.LogWarning($"[Tutorial] firstKey 파싱 실패 - 종료 처리: {e.Message}");
				isEnd = true;
			}
		}

		List<Param> newParams = new List<Param>();

		string[] allTutoIndates = new string[]
		{
			"start", "inven", "staff", "hunter", "store", "enhance", "dispatch",
		};

		foreach (var indate in allTutoIndates)
		{
			if (!existingIndates.Contains(indate))
			{
				Param param = new Param();
				param.Add("owner_inDate", ownerIndate);
				param.Add(allTutoIndates, false);
				newParams.Add(param);
			}
		}

		foreach (var param in newParams)
		{
			bool isInsertDone = false;
			BackendReturnObject insertBro = null;

			Backend.GameData.Insert("Tutorial", param, callback =>
			{
				insertBro = callback;
				isInsertDone = true;
			});

			yield return new WaitUntil(() => isInsertDone);

			if (!insertBro.IsSuccess())
			{
				Debug.LogError($"[Insert 실패]");
			}
			else
			{
				Debug.Log($"[Insert 성공]");
			}
		}

		Debug.Log("[Tutorial] 튜토리얼 항목 삽입 완료");
	}

	public IEnumerator LoadUserTuto(string ownerIndate)
	{
		string firstKey = null;
		const int limit = 100;
		bool isEnd = false;

		while (!isEnd)
		{
			bool isDone = false;
			BackendReturnObject bro = null;

			var where = new Where();
			where.Equal("owner_inDate", ownerIndate);

			if (string.IsNullOrEmpty(firstKey))
			{
				Backend.GameData.Get("Tutorial", where, limit, callback =>
				{
					bro = callback;
					isDone = true;
				});
			}
			else
			{
				Backend.GameData.Get("Tutorial", where, limit, firstKey, callback =>
				{
					bro = callback;
					isDone = true;
				});
			}

			yield return new WaitUntil(() => isDone);

			if (bro == null || !bro.IsSuccess())
			{
				Debug.LogError($"[ERROR] Tutorial 서버 데이터 로드 실패: {bro}");
				yield break;
			}

			var rows = bro.FlattenRows();

			foreach (LitJson.JsonData row in rows)
			{
				// 파싱
				//string itemName = row["inventoryItemName"].ToString();
				//int quantity = int.Parse(row["inventoryQuantity"].ToString());

				tutorialBool.Instance.clearStartTuto = bool.Parse(row["start"].ToString());
				tutorialBool.Instance.clearInvenTuto = bool.Parse(row["inven"].ToString());
				tutorialBool.Instance.clearBuyStaffTuto = bool.Parse(row["staff"].ToString());
				tutorialBool.Instance.clearBuyHunterTuto = bool.Parse(row["hunter"].ToString());
				tutorialBool.Instance.clearShopTuto = bool.Parse(row["store"].ToString());
				tutorialBool.Instance.clearDispatchTuto = bool.Parse(row["dispatch"].ToString());
				tutorialBool.Instance.clearLevelUpTuto = bool.Parse(row["enhance"].ToString());

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
				Debug.LogWarning($"[Tutorial] firstKey 파싱 실패 -> 종료 처리: {e.Message}");
				isEnd = true;
			}
		}

		inventoryLoaded = true;

		OnInventoryChanged += AutoSave;
		AutoSaveManager.Instance?.RegisterAutoSavable(this);

		Debug.Log("[Tutorial] 유저 튜토리얼 데이터 로드 완료");
	}

	public void SaveTuto(System.Action onComplete = null)
	{
		if (!inventoryLoaded)
		{
			Debug.LogWarning("[저장 차단] 튜토리얼 로딩 안 끝났음");
			onComplete?.Invoke();
			return;
		}

		string ownerIndate = Backend.UserInDate;

		Param param = new Param();
		param.Add("start", tutorialBool.Instance.clearStartTuto);
		param.Add("inven", tutorialBool.Instance.clearInvenTuto);
		param.Add("staff", tutorialBool.Instance.clearBuyStaffTuto);
		param.Add("hunter", tutorialBool.Instance.clearBuyHunterTuto);
		param.Add("store", tutorialBool.Instance.clearShopTuto);
		param.Add("dispatch", tutorialBool.Instance.clearDispatchTuto);
		param.Add("enhance", tutorialBool.Instance.clearLevelUpTuto);
		Where where = new Where();
		where.Equal("owner_inDate", ownerIndate);

		Backend.GameData.Update("Tutorial", where, param, bro =>
		{
			if (bro.IsSuccess())
			{
				Debug.Log($"[튜토리얼 저장 성공]");
			}
			else
			{
				Debug.LogError($"[튜토리얼 저장 실패] / {bro}");
			}
			onComplete?.Invoke();
		});


		//int dirtyCount = 0;

		//for (int i = 0; i < allRunTimeIngredients.Length; i++)
		//{
		//	var runtimeData = allRunTimeIngredients[i];
		//	if (!runtimeData.isDirty) continue;
		//
		//	dirtyCount++;
		//
		//	string itemIndate = allIngredients[i].indate;
		//	int qty = runtimeData.ingredientQty;
		//
		//	Where where = new Where();
		//	where.Equal("owner_inDate", ownerIndate);
		//	where.Equal("inventoryItemIndate", itemIndate);
		//
		//	Param param = new Param();
		//	param.Add("inventoryQuantity", qty);
		//
		//	// 비동기 저장 처리
		//	Backend.GameData.Update("INVENTORY", where, param, bro =>
		//	{
		//		if (bro.IsSuccess())
		//		{
		//			runtimeData.isDirty = false;
		//			//Debug.Log($"[인벤토리 저장 성공] {itemIndate} : {qty}");
		//		}
		//		else
		//		{
		//			Debug.LogError($"[인벤토리 저장 실패] {itemIndate} : {qty} / {bro}");
		//		}
		//
		//		finishedCount++;
		//
		//		if (finishedCount >= dirtyCount)
		//		{
		//			Debug.Log("모든 변경된 인벤토리 저장 완료");
		//			onComplete?.Invoke();
		//		}
		//	});
		//}

		// 변경 사항 없을 경우 바로 콜백 호출
		//if (dirtyCount == 0)
		//{
		//	//Debug.Log("변경된 인벤토리 없음 -> 저장 생략");
		//	onComplete?.Invoke();
		//}
	}

	private bool inventoryLoaded = false;
}
