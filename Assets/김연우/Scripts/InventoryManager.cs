using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    // �����Ϳ��� �巡���� ��� IngredientData ���
    [Header("Config: ��� ��� ������")]
    public IngredientData[] allIngredients;

    [Header("Runtime: �κ��丮 ����")]
    public List<InventorySlot> slots = new List<InventorySlot>();

    // �κ��丮 ���� �̺�Ʈ
    public event Action OnInventoryChanged;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        // �� �ʱ� ���� ����
        foreach (var data in allIngredients)
        {
            if (data.qty <= 0) continue;
            slots.Add(new InventorySlot(data));
        }


        OnInventoryChanged?.Invoke();
    }

    /// <summary>
    /// ������ �߰�
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
    /// ������ ����
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
    /// ��ü ����
    /// </summary>
    public void ClearInventory()
    {
        slots.Clear();
        OnInventoryChanged?.Invoke();
    }
}
