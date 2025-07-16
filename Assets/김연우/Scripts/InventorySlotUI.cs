using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlotUI : MonoBehaviour
{
    [Header("UI References")]
    public Image iconImage;
    public TMP_Text qtyText;

    private InventorySlot slot;
    private int lastQty = -1;

    public void SetSlot(InventorySlot slot)
    {
        this.slot = slot;

        // 아이콘 세팅
        if (iconImage != null)
            iconImage.sprite = Resources.Load<Sprite>(slot.ingredient.ingredientSprite);

        // 초기 수량 표시
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
}
