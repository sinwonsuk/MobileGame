// InventorySlotUI.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlotUI : MonoBehaviour
{
    [Header("UI References")]
    public Image iconImage;   // Prefab ��忡�� �巡�� ������ �ּ��� (Icon)
    public TMP_Text qtyText;     // �ڵ� ���ε� �Ǵ� Prefab ��忡�� �巡��

    private InventorySlot slot;

    private void Awake()
    {
        // ing_count��� �̸����� �ڽĿ� ���� TMP_Text�� �ڵ����� ã�Ƽ� ����
        if (qtyText == null)
        {
            var t = transform.Find("ing_count");
            if (t != null)
                qtyText = t.GetComponent<TMP_Text>();
            else
                Debug.LogWarning($"[{name}] �ڽĿ� 'ing_count' ������Ʈ�� �����ϴ�.");
        }

        // iconImage�� �ڵ� ���ε��� ���ϸ� �Ʒ�ó�� �߰� ����
        // if (iconImage == null)
        //     iconImage = transform.Find("Icon").GetComponent<Image>();
    }

    /// <summary>
    /// ���� �����͸� �޾� ȭ�鿡 ǥ��
    /// </summary>
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
