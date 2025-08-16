using System.Collections;
using UnityEngine;

public class EmployeeInventoryUI : MonoBehaviour
{
    public GameObject slotPrefab;

    // 기존 contentParent 대신 두 개로 분리
    public Transform hunterParent;       // = HunterInnerGrid
    public Transform restaurantParent;   // = RestaurantInnerGrid

    public EmployeeDetailPanel detailPanel;

    private void Start()
    {
        RefreshUI();
        EmployeeSlotUI.OnSlotClicked += OnSlotClicked;

        if (EmployeeManager.Instance != null)
            EmployeeManager.Instance.OnStaffChanged += RefreshUI;
        else
            StartCoroutine(WaitForManagerThenSubscribe());
    }

    private IEnumerator WaitForManagerThenSubscribe()
    {
        while (EmployeeManager.Instance == null)
            yield return null;
        EmployeeManager.Instance.OnStaffChanged += RefreshUI;
    }

    private void OnDestroy()
    {
        EmployeeSlotUI.OnSlotClicked -= OnSlotClicked;
        if (EmployeeManager.Instance != null)
            EmployeeManager.Instance.OnStaffChanged -= RefreshUI;
    }

    public void RefreshUI()
    {
        // 각각 비우기
        foreach (Transform c in hunterParent) Destroy(c.gameObject);
        foreach (Transform c in restaurantParent) Destroy(c.gameObject);

        // 직원 슬롯 다시 채우기
        foreach (var slot in EmployeeManager.Instance.slots)
        {
            if (!slot.IsOwned) continue; // 소유한 직원만 표시

            Transform parent = null;
            switch (slot.staffData.staffType)
            {
                case StaffType.hunter:      parent = hunterParent; break;
                case StaffType.restaurant:  parent = restaurantParent; break;
                default: continue;
            }

            var go = Instantiate(slotPrefab, parent);
            go.GetComponent<EmployeeSlotUI>().SetSlot(slot);
        }
    }

    private void OnEnable() { StartCoroutine(SubscribeWhenReady()); }
    private IEnumerator SubscribeWhenReady()
    {
        while (EmployeeManager.Instance == null)
            yield return null;
        EmployeeManager.Instance.OnStaffChanged += RefreshUI;
    }
    private void OnDisable()
    {
        if (EmployeeManager.Instance != null)
            EmployeeManager.Instance.OnStaffChanged -= RefreshUI;
    }

    private void OnSlotClicked(EmployeeSlot slot)
    {
        detailPanel.Open(slot);
    }
}
