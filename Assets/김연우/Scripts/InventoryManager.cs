using System;
using System.Collections.Generic;
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
            // if (allRunTimeIngredients[i].ingredientQty <= 0) continue;
            slots.Add(new InventorySlot(allIngredients[i], allRunTimeIngredients[i]));
        }

        OnInventoryChanged?.Invoke();
    }

    /// <summary>
    /// 재료 추가
    /// </summary>
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

    public void RemoveItem(string name, int amount = 1)
    {
        foreach (var data in allIngredients)
        {
            if (data.ingredientName == name)
            {
                // data.qty -= amount;
                return;
            }
        }

        OnInventoryChanged?.Invoke();
    }

    /// <summary>
    /// 전체 초기화
    /// </summary>
    public void ClearInventory()
    {
        slots.Clear();
        OnInventoryChanged?.Invoke();
    }
}
