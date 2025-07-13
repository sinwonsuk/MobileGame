using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlotUI : MonoBehaviour
{
    [Header("UI References")]
    public Image iconImage; 
    public TMP_Text qtyText; 

    private InventorySlot slot;

    private void Awake()
    {
        if (qtyText == null)
        {
            var t = transform.Find("ing_count");
            if (t != null)
                qtyText = t.GetComponent<TMP_Text>();
            else
                Debug.LogWarning($"[{name}] 자식에 'ing_count' 오브젝트가 없습니다.");
        }

        // iconImage 자동 바인딩
        // if (iconImage == null)
        //     iconImage = transform.Find("Icon").GetComponent<Image>();
    }

    public void SetSlot(InventorySlot slot)
    {
        this.slot = slot;

        // 아이콘 세팅
        if (iconImage != null)
            iconImage.sprite = Resources.Load<Sprite>(slot.ingredient.ingredientSprite);

        // 수량 텍스트 세팅
        if (qtyText != null)
            qtyText.text = slot.quantity.ToString();
    }
}
