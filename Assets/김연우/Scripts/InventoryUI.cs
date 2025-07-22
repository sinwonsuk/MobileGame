using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [Header("UI Slot Prefab")]
    public GameObject slotPrefab;
    public Transform contentParent;

    private void Start()
    {
        gameObject.SetActive(false);
        // 필드 할당 체크
        if (slotPrefab == null || contentParent == null)
        {
            Debug.LogError("[InventoryUI] slotPrefab 혹은 contentParent가 할당되지 않았습니다.", this);
            enabled = false;
            return;
        }

        // 인벤토리 변경 이벤트 구독
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnInventoryChanged += RefreshUI;
        }
        else
        {
            Debug.LogError("[InventoryUI] InventoryManager.Instance가 null입니다!", this);
            enabled = false;
            return;
        }

        // 초기 UI 생성
        RefreshUI();
    }


    private void OnDestroy()
    {
        // 안전하게 해제
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnInventoryChanged -= RefreshUI;

        EventBus<ToggleInventoryEvent>.OnEvent -= OnToggleInventory;
    }

    private void RefreshUI()
    {
        // 더블 체크: 혹시 호출된 상태라면 다시 한번 널 방어
        if (contentParent == null || slotPrefab == null || InventoryManager.Instance == null)
            return;

        // 기존 슬롯 제거
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        // 수량>0인 슬롯만 다시 생성
        foreach (var slot in InventoryManager.Instance.slots)
        {
            if (slot.runTimeIngredientData.ingredientQty <= 0)
                continue;

            var go = Instantiate(slotPrefab, contentParent);
            go.GetComponent<InventorySlotUI>().SetSlot(slot);
        }
    }
    private void OnToggleInventory(ToggleInventoryEvent evt)
    {
        // 꺼져있으면 켜고, 켜져있으면 끄기
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
