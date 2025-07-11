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
                Debug.LogWarning($"[{name}] �ڽĿ� 'ing_count' ������Ʈ�� �����ϴ�.");
        }

        // iconImage �ڵ� ���ε�
        // if (iconImage == null)
        //     iconImage = transform.Find("Icon").GetComponent<Image>();
    }

    public void SetSlot(InventorySlot slot)
    {
        this.slot = slot;

        // ������ ����
        if (iconImage != null)
            iconImage.sprite = Resources.Load<Sprite>(slot.ingredient.ingredientSprite);

        // ���� �ؽ�Ʈ ����
        if (qtyText != null)
            qtyText.text = slot.quantity.ToString();
    }
}
