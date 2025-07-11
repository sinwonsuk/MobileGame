// InventoryUI.cs
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [Header("UI Slot Prefab")]
    public GameObject slotPrefab;
    public Transform contentParent;

    private void OnEnable()
    {
        InventoryManager.Instance.OnInventoryChanged += RefreshUI;
        RefreshUI();
    }

    private void OnDisable()
    {
        InventoryManager.Instance.OnInventoryChanged -= RefreshUI;
    }

    /// <summary>
    /// UI 갱신
    /// </summary>
    // InventoryUI.cs
    private void RefreshUI()
    {
        // 기존 UI 제거
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        // slots 리스트를 순회하면서
        foreach (var slot in InventoryManager.Instance.slots)
        {
            // ★ 수량이 0이면 생성하지 않는다
            if (slot.quantity <= 0)
                continue;

            // 수량이 1 이상인 재료만 Instantiate
            var go = Instantiate(slotPrefab, contentParent);
            var slotUI = go.GetComponent<InventorySlotUI>();
            slotUI.SetSlot(slot);
        }
    }

}
