using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlotUI : MonoBehaviour
{
    [Header("UI References")]
    public Image iconImage;
    public TMP_Text qtyText;

    private InventorySlot slot;

    public void SetSlot(InventorySlot slot)
    {
        this.slot = slot;

        // 아이콘 세팅 (ingredientSprite는 SO에 저장된 경로)
        if (iconImage != null)
            iconImage.sprite = Resources.Load<Sprite>(slot.ingredient.ingredientSprite);

        // ★ 수량은 런타임 SO
        if (qtyText != null)
            qtyText.text = slot.runTimeIngredientData.ingredientQty.ToString();
    }
}
