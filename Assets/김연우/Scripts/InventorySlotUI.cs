using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;     // ← 추가
using System;                      // for Action<>

public class InventorySlotUI : MonoBehaviour, IPointerClickHandler
{
    [Header("UI References")]
    public Image iconImage;
    public TMP_Text qtyText;

    private InventorySlot slot;
    private int lastQty = -1;

    // 클릭된 슬롯을 외부로 브로드캐스트
    public static event Action<InventorySlot> OnSlotClicked;

    public void SetSlot(InventorySlot slot)
    {
        this.slot = slot;
        if (iconImage != null)
            iconImage.sprite = Resources.Load<Sprite>(slot.ingredient.ingredientSprite);
        UpdateQty(slot.runTimeIngredientData.ingredientQty);
    }

    void Update()
    {
        if (slot == null) return;
        int current = slot.runTimeIngredientData.ingredientQty;
        if (current != lastQty)
            UpdateQty(current);
    }

    private void UpdateQty(int qty)
    {
        lastQty = qty;
        if (qtyText != null)
            qtyText.text = qty.ToString();
    }

    // ← 슬롯 클릭 시 이벤트 발송
    public void OnPointerClick(PointerEventData eventData)
    {
        if (slot != null)
            OnSlotClicked?.Invoke(slot);
    }
}
