using System.Collections;
using UnityEngine;

public class EmployeeInventoryUI : MonoBehaviour
{
    public GameObject slotPrefab;
    public Transform contentParent;
    public EmployeeDetailPanel detailPanel;

    private void Start()
    {
        // 초기화 시 한 번만 새로고침
        RefreshUI();

        // 슬롯 클릭 이벤트 구독
        EmployeeSlotUI.OnSlotClicked += OnSlotClicked;

        // 직원 데이터 변경 시 자동 새로고침 이벤트 구독 (이벤트 패턴)
        if (EmployeeManager.Instance != null)
            EmployeeManager.Instance.OnStaffChanged += RefreshUI;
        else
            StartCoroutine(WaitForManagerThenSubscribe());
    }

    private IEnumerator WaitForManagerThenSubscribe()
    {
        // EmployeeManager.Instance 생성이 늦을 수 있으니 기다렸다가 구독
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
    foreach (Transform child in contentParent)
        Destroy(child.gameObject);

    foreach (var slot in EmployeeManager.Instance.slots) // 슬롯 목록
    {
        if (!slot.IsOwned) continue;


        var t = slot.staffData.staffType;
        if (t == StaffType.hunter || t == StaffType.restaurant)
        {
            var go = Instantiate(slotPrefab, contentParent);
            go.GetComponent<EmployeeSlotUI>().SetSlot(slot);
        }
    }
}

    private void OnEnable()
    {
        StartCoroutine(SubscribeWhenReady());
    }
    private IEnumerator SubscribeWhenReady()
    {
        while (EmployeeManager.Instance == null)
            yield return null;
        EmployeeManager.Instance.OnStaffChanged += RefreshUI;
        Debug.Log("EmployeeInventoryUI: 이벤트 구독 성공!");
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
