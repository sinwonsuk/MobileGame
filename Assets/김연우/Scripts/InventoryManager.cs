using BackEnd;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [Header("Config: 전체 재료 데이터")]
    public IngredientData[] allIngredients;

    [Header("Config: 전체 런타임 재료 데이터")]
    public RunTimeIngredientData[] allRunTimeIngredients;

    [Header("Runtime: 인벤토리 슬롯")]
    public List<InventorySlot> slots = new List<InventorySlot>();

    public event Action OnInventoryChanged;

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
        OnInventoryChanged?.Invoke();
    }

    public void AddItem(string name, int amount = 1)
    {
        foreach (var data in allRunTimeIngredients)
        {
            if (data.ingredientName == name)
            {
                data.ingredientQty += amount;
                return;
            }
        }
        OnInventoryChanged?.Invoke();
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
                return data.ingredientQty.ToString();
            }
        }
        OnInventoryChanged?.Invoke();
        return "0";
    }

    public string DecreaseQty(string name, int amount = 1)
    {
        foreach (var data in allRunTimeIngredients)
        {
            if (data.ingredientName == name)
            {
                data.ingredientQty -= amount;
                return data.ingredientQty.ToString();
            }
        }
        OnInventoryChanged?.Invoke();
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
		HashSet<string> existingItems = new HashSet<string>();

		while (!isEnd)
		{
			bool isDone = false;
			BackendReturnObject bro = null;

			var where = new Where();
			where.Equal("ownerIndate", ownerIndate);
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
			foreach (LitJson.JsonData row in rows)
			{
				existingItems.Add(row["inventoryItemName"].ToString());
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

			if (!existingItems.Contains(runtimeData.ingredientName))
			{
				Param param = new Param();
				param.Add("inventoryItemIndate", staticData.indate);
				param.Add("inventoryItemType", "Ingredient");
				param.Add("inventoryItemName", runtimeData.ingredientName);
				param.Add("inventoryQuantity", 0);

				newParams.Add(param);
			}
		}

		// 하나씩 Insert 호출
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
			where.Equal("ownerIndate", ownerIndate);
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

		OnInventoryChanged?.Invoke();
	}
}
