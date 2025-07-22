using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [Header("UI Slot Prefab")]
    public GameObject slotPrefab;      // → 반드시 인스펙터에 할당!
    public Transform contentParent;    // → 반드시 인스펙터에 할당!

    private void Start()
    {
        // 1) Inspector 참조 체크
        if (slotPrefab == null || contentParent == null)
        {
            Debug.LogError("[InventoryUI] slotPrefab 혹은 contentParent가 할당되지 않았습니다.", this);
            enabled = false; // 이 컴포넌트 비활성화
            return;
        }

        // 2) 싱글턴 체크 & 이벤트 구독
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

        // 3) 첫 화면 렌더
        RefreshUI();
    }

    private void OnDestroy()
    {
        // 안전하게 해제
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnInventoryChanged -= RefreshUI;
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
}
