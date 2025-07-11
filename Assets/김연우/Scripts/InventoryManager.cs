using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    // 에디터에서 드래그할 모든 IngredientData 목록
    [Header("Config: 모든 재료 데이터")]
    public IngredientData[] allIngredients;

    [Header("Runtime: 인벤토리 슬롯")]
    public List<InventorySlot> slots = new List<InventorySlot>();

    // 인벤토리 변경 이벤트
    public event Action OnInventoryChanged;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        // ① 초기 슬롯 세팅
        foreach (var data in allIngredients)
        {
            if (data.qty <= 0) continue;
            slots.Add(new InventorySlot(data));
        }


        OnInventoryChanged?.Invoke();
    }

    /// <summary>
    /// 아이템 추가
    /// </summary>
    public void AddItem(IngredientData data, int amount = 1)
    {
        var slot = slots.Find(s => s.ingredient == data);
        if (slot != null)
            slot.quantity += amount;
        else
        {
            var newSlot = new InventorySlot(data);
            newSlot.quantity = amount;
            slots.Add(newSlot);
        }
        OnInventoryChanged?.Invoke();
    }

    /// <summary>
    /// 아이템 제거
    /// </summary>
    public void RemoveItem(IngredientData data, int amount = 1)
    {
        var slot = slots.Find(s => s.ingredient == data);
        if (slot == null) return;

        slot.quantity -= amount;
        if (slot.quantity <= 0)
            slots.Remove(slot);

        OnInventoryChanged?.Invoke();
    }

    /// <summary>
    /// 전체 비우기
    /// </summary>
    public void ClearInventory()
    {
        slots.Clear();
        OnInventoryChanged?.Invoke();
    }
}
