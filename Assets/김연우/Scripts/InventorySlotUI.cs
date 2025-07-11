// InventorySlotUI.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlotUI : MonoBehaviour
{
    [Header("UI References")]
    public Image iconImage;   // Prefab 모드에서 드래그 연결해 주세요 (Icon)
    public TMP_Text qtyText;     // 자동 바인딩 또는 Prefab 모드에서 드래그

    private InventorySlot slot;

    private void Awake()
    {
        // ing_count라는 이름으로 자식에 붙은 TMP_Text를 자동으로 찾아서 연결
        if (qtyText == null)
        {
            var t = transform.Find("ing_count");
            if (t != null)
                qtyText = t.GetComponent<TMP_Text>();
            else
                Debug.LogWarning($"[{name}] 자식에 'ing_count' 오브젝트가 없습니다.");
        }

        // iconImage도 자동 바인딩을 원하면 아래처럼 추가 가능
        // if (iconImage == null)
        //     iconImage = transform.Find("Icon").GetComponent<Image>();
    }

    /// <summary>
    /// 슬롯 데이터를 받아 화면에 표시
    /// </summary>
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
