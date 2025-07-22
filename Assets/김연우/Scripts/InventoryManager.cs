using BackEnd;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryManager : MonoBehaviour, IAutoSavable
{
    public static InventoryManager Instance { get; private set; }

    [Header("Config: 전체 재료 데이터")]
    public IngredientData[] allIngredients;

    [Header("Config: 전체 런타임 재료 데이터")]
    public RunTimeIngredientData[] allRunTimeIngredients;

    [Header("Runtime: 인벤토리 슬롯")]
    public List<InventorySlot> slots = new List<InventorySlot>();

    public event Action OnInventoryChanged;

	public void AutoSave()
	{
		if (inventoryLoaded)
			SaveImmediately();
		else
			Debug.LogWarning("인벤토리 로딩 안 됨 -> 종료 시 저장 생략");

		SaveInventory();
	}

	private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

		// 초기 슬롯 세팅

		for (int i = 0; i < allIngredients.Length; i++)
        {
            slots.Add(new InventorySlot(allIngredients[i], allRunTimeIngredients[i]));
        }
	}

    public void AddItem(string name, int amount = 1)
    {
        foreach (var data in allRunTimeIngredients)
        {
            if (data.ingredientName == name)
            {
                data.ingredientQty += amount;
				data.isDirty = true;
				OnInventoryChanged?.Invoke();
                return;
            }
        }
    }

    public int GetItemQty(string name)
    {
        if (name == "")
            return -1;

        foreach (var data in allRunTimeIngredients)
        {
            if (data.ingredientName == name)
            {
                return data.ingredientQty;
            }
        }

        return 0; // 해당 재료가 없을 경우 0 반환
    }

    public string IncreaseQty(string name, int amount = 1)
    {
        foreach (var data in allRunTimeIngredients)
        {
            if (data.ingredientName == name)
            {
                data.ingredientQty += amount;
				data.isDirty = true;
				OnInventoryChanged?.Invoke();
                return data.ingredientQty.ToString();
            }
        }
        return "0";
    }

    public string DecreaseQty(string name, int amount = 1)
    {
        foreach (var data in allRunTimeIngredients)
        {
            if (data.ingredientName == name)
            {
                data.ingredientQty -= amount;
				data.isDirty = true;
				OnInventoryChanged?.Invoke();
                return data.ingredientQty.ToString();
            }
        }
        return "0";
    }

    /// <summary>
    /// 전체 초기화
    /// </summary>
    public void ClearInventory()
    {
        slots.Clear();
        OnInventoryChanged?.Invoke();
    }

	public IEnumerator InsertInventoryIfNotExists(string ownerIndate)
	{
		string offset = "";
		const int limit = 100;
		bool isEnd = false;

		HashSet<string> existingIndates = new HashSet<string>();

		while (!isEnd)
		{
			bool isDone = false;
			BackendReturnObject bro = null;

			var where = new Where();
			where.Equal("owner_inDate", ownerIndate);
			//Debug.Log("[확인용] 현재 ownerIndate: " + ownerIndate);
			where.Equal("inventoryItemType", "Ingredient");

			Backend.GameData.Get("INVENTORY", where, limit, offset, callback =>
			{
				bro = callback;
				isDone = true;
			});

			yield return new WaitUntil(() => isDone);

			if (bro == null || !bro.IsSuccess())
			{
				Debug.LogError($"[ERROR] INVENTORY 서버 조회 실패: {bro}");
				yield break;
			}


			var rows = bro.FlattenRows();

			if (rows == null || rows.Count == 0)
			{
				Debug.LogWarning("[InsertInventoryIfNotExists] 서버에서 받은 INVENTORY row 없음!");
			}

			//foreach (LitJson.JsonData row in rows)
			//{
			//	Debug.Log("[Debug 구조 확인] row: " + row.ToJson());
			//}

			foreach (LitJson.JsonData row in rows)
			{
				if (row.ContainsKey("inventoryItemIndate"))
				{
					string itemIndate = row["inventoryItemIndate"].ToString().Trim();
					existingIndates.Add(itemIndate);
					//Debug.Log($"[중복 체크용] 기존 indate: {itemIndate}");
				}
				else
				{
					Debug.LogError("[InsertInventoryIfNotExists] inventoryItemIndate 필드가 누락됨 — 데이터 구조 점검 필요");
				}
			}

			try
			{
				var json = LitJson.JsonMapper.ToObject(bro.GetReturnValue());
				if (json.ContainsKey("offset"))
					offset = json["offset"].ToString();
				else
					isEnd = true;
			}
			catch (Exception e)
			{
				Debug.LogWarning($"[Inventory] offset 파싱 실패 → 종료 처리: {e.Message}");
				isEnd = true;
			}
		}

		List<Param> newParams = new List<Param>();

		for (int i = 0; i < allRunTimeIngredients.Length; i++)
		{
			var runtimeData = allRunTimeIngredients[i];
			var staticData = allIngredients[i];

			if (!existingIndates.Contains(staticData.indate))
			{
				Param param = new Param();
				param.Add("inventoryItemIndate", staticData.indate);
				param.Add("inventoryItemType", "Ingredient");
				param.Add("inventoryItemName", runtimeData.ingredientName);
				param.Add("inventoryQuantity", 0);

				newParams.Add(param);
			}
		}

		foreach (var param in newParams)
		{
			bool isInsertDone = false;
			BackendReturnObject insertBro = null;

			Backend.GameData.Insert("INVENTORY", param, callback =>
			{
				insertBro = callback;
				isInsertDone = true;
			});

			yield return new WaitUntil(() => isInsertDone);

			if (!insertBro.IsSuccess())
			{
				Debug.LogError($"[Insert 실패] {param["inventoryItemName"]}: {insertBro.GetMessage()}");
			}
			else
			{
				Debug.Log($"[Insert 성공] {param["inventoryItemName"]}");
			}
		}

		Debug.Log("[Inventory] 신규 항목 삽입 완료");
	}


	public IEnumerator LoadUserInventory(string ownerIndate)
	{
		string offset = "";
		const int limit = 100;
		bool isEnd = false;

		while (!isEnd)
		{
			bool isDone = false;
			BackendReturnObject bro = null;

			var where = new Where();
			where.Equal("owner_inDate", ownerIndate);
			where.Equal("inventoryItemType", "Ingredient");

			Backend.GameData.Get("INVENTORY", where, limit, offset, callback =>
			{
				bro = callback;
				isDone = true;
			});

			yield return new WaitUntil(() => isDone);

			if (bro == null || !bro.IsSuccess())
			{
				Debug.LogError($"[ERROR] INVENTORY 서버 데이터 로드 실패: {bro}");
				yield break;
			}

			var rows = bro.FlattenRows();

			foreach (LitJson.JsonData row in rows)
			{
				string itemName = row["inventoryItemName"].ToString();
				int quantity = int.Parse(row["inventoryQuantity"].ToString());

				var target = allRunTimeIngredients.FirstOrDefault(i => i.ingredientName == itemName);
				if (target != null)
					target.ingredientQty = quantity;
			}

			try
			{
				var json = LitJson.JsonMapper.ToObject(bro.GetReturnValue());
				if (json.ContainsKey("offset"))
				{
					offset = json["offset"].ToString();
				}
				else
				{
					isEnd = true;
				}
			}
			catch (Exception e)
			{
				Debug.LogWarning($"[Inventory] offset 파싱 실패 -> 종료 처리: {e.Message}");
				isEnd = true;
			}
		}

		inventoryLoaded = true;

		OnInventoryChanged += AutoSave;
		AutoSaveManager.Instance?.RegisterAutoSavable(this);

		Debug.Log("[Inventory] 유저 인벤토리 데이터 로드 완료");
	}

	public void SaveInventory()
	{
		if (!inventoryLoaded)
		{
			Debug.LogWarning("[저장 차단] 인벤토리 로딩 안 끝났음");
			return;
		}

		string ownerIndate = Backend.UserInDate;

		for (int i = 0; i < allRunTimeIngredients.Length; i++)
		{
			var runtimeData = allRunTimeIngredients[i];
			if (!runtimeData.isDirty) continue;

			string itemIndate = allIngredients[i].indate;
			int qty = allRunTimeIngredients[i].ingredientQty;

			Where where = new Where();
			where.Equal("owner_inDate", ownerIndate);
			where.Equal("inventoryItemIndate", itemIndate);

			Param param = new Param();
			param.Add("inventoryQuantity", qty);

			Backend.GameData.Update("INVENTORY", where, param);

			runtimeData.isDirty = false;
		}

		Debug.Log("변경된 인벤토리만 자동 저장 완료");
	}

	private void OnApplicationQuit()
	{
		if (inventoryLoaded)
			SaveImmediately();
		else
			Debug.LogWarning("인벤토리 로딩 안 됨 -> 종료 시 저장 생략");
	}

	private void OnApplicationPause(bool pause)
	{
		if (pause && inventoryLoaded)
		{
			Debug.Log("일시 정지 시 인벤토리 저장");
			SaveImmediately();
		}
	}

	private void OnApplicationFocus(bool hasFocus)
	{
		if (!hasFocus && inventoryLoaded)
		{
			Debug.Log("포커스 잃을 시 인벤토리 저장");
			SaveImmediately();
		}
	}


	private void SaveImmediately()
	{
		string ownerIndate = Backend.UserInDate;

		for (int i = 0; i < allRunTimeIngredients.Length; i++)
		{
			var runtimeData = allRunTimeIngredients[i];
			if (!runtimeData.isDirty) continue;

			string itemIndate = allIngredients[i].indate;
			int qty = allRunTimeIngredients[i].ingredientQty;

			Where where = new Where();
			where.Equal("owner_inDate", ownerIndate);
			where.Equal("inventoryItemIndate", itemIndate);

			Param param = new Param();
			param.Add("inventoryQuantity", qty);

			Backend.GameData.Update("INVENTORY", where, param);

			runtimeData.isDirty = false;
		}

		Debug.Log("종료 시 변경된 인벤 데이터 저장 완료");
	}

	private bool inventoryLoaded = false;
}
