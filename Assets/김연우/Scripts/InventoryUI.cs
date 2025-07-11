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
    /// UI ����
    /// </summary>
    // InventoryUI.cs
    private void RefreshUI()
    {
        // ���� UI ����
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        // slots ����Ʈ�� ��ȸ�ϸ鼭
        foreach (var slot in InventoryManager.Instance.slots)
        {
            // �� ������ 0�̸� �������� �ʴ´�
            if (slot.quantity <= 0)
                continue;

            // ������ 1 �̻��� ��Ḹ Instantiate
            var go = Instantiate(slotPrefab, contentParent);
            var slotUI = go.GetComponent<InventorySlotUI>();
            slotUI.SetSlot(slot);
        }
    }

}
