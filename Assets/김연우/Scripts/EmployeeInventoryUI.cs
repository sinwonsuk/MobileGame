using UnityEngine;

public class EmployeeInventoryUI : MonoBehaviour
{
    public GameObject slotPrefab;
    public Transform contentParent;
    public EmployeeDetailPanel detailPanel;

    private void Start()
    {
        // EmployeePanel = this.transform이므로, 바로 자식 찾기

        // 기존 로직
        RefreshUI();
        EmployeeSlotUI.OnSlotClicked += OnSlotClicked;
    }

    private void OnDestroy()
    {
        EmployeeSlotUI.OnSlotClicked -= OnSlotClicked;
    }

    public void RefreshUI()
    {
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        foreach (var slot in EmployeeManager.Instance.slots)
        {
            if (slot.IsOwned)
            {
                var go = Instantiate(slotPrefab, contentParent);
                go.GetComponent<EmployeeSlotUI>().SetSlot(slot);
            }
        }
    }

    private void OnSlotClicked(EmployeeSlot slot)
    {
        detailPanel.Open(slot);
    }
}
