using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems;

public class SellPanelUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image previewImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private TMP_Text totalText;
    [SerializeField] private Button leftBtn;
    [SerializeField] private Button rightBtn;
    [SerializeField] private TMP_InputField qtyInput;
    [SerializeField] private Button sellBtn;

    private InventorySlot currentSlot;
    private int sellQty;

    void Awake()
    {
        // 씬 로드 직후에 한 번만 구독
        InventorySlotUI.OnSlotClicked += ShowPanel;
    }

    void OnDestroy()
    {
        // 오브젝트가 파괴될 때 정리
        InventorySlotUI.OnSlotClicked -= ShowPanel;
    }

    void OnEnable()
    {
        InventorySlotUI.OnSlotClicked += ShowPanel;
        leftBtn.onClick.AddListener(OnLeft);
        rightBtn.onClick.AddListener(OnRight);
        qtyInput.onValueChanged.AddListener(OnInputChanged);
        sellBtn.onClick.AddListener(OnSell);
    }

    void OnDisable()
    {
        InventorySlotUI.OnSlotClicked -= ShowPanel;
        leftBtn.onClick.RemoveAllListeners();
        rightBtn.onClick.RemoveAllListeners();
        qtyInput.onValueChanged.RemoveAllListeners();
        sellBtn.onClick.RemoveAllListeners();
    }

    private void ShowPanel(InventorySlot slot)
    {
        Debug.Log("[SellPanelUI] 슬롯 클릭 감지 → 패널 띄우기", this);
        currentSlot = slot;
        sellQty = 1;
        previewImage.sprite = Resources.Load<Sprite>(slot.ingredient.ingredientSprite);
        nameText.text = slot.ingredient.ingredientName;
        priceText.text = slot.ingredient.ingredientPrice.ToString();
        qtyInput.text = sellQty.ToString();
        UpdateTotal();
        gameObject.SetActive(true);
    }

    private void OnLeft()
    {
        if (sellQty > 1)
        {
            sellQty--;
            qtyInput.text = sellQty.ToString();
            UpdateTotal();
        }
    }

    private void OnRight()
    {
        int max = currentSlot.runTimeIngredientData.ingredientQty;
        if (sellQty < max)
        {
            sellQty++;
            qtyInput.text = sellQty.ToString();
            UpdateTotal();
        }
    }

    private void OnInputChanged(string s)
    {
        if (int.TryParse(s, out int v))
        {
            int max = currentSlot.runTimeIngredientData.ingredientQty;
            sellQty = Mathf.Clamp(v, 1, max);
            qtyInput.text = sellQty.ToString();
            UpdateTotal();
        }
        else
        {
            qtyInput.text = sellQty.ToString();
        }
    }

    private void UpdateTotal()
    {
        int total = sellQty * currentSlot.ingredient.ingredientPrice;
        totalText.text = total.ToString();
    }

    private void OnSell()
    {
        // 1) 인벤토리에서 수량 차감 이벤트
        InventoryManager.Instance.DecreaseQty(currentSlot.ingredient.ingredientName, sellQty);

        // 돈 획득 이벤트
        int gain = sellQty * currentSlot.ingredient.ingredientPrice;
        //EventBus<MoneyChangePusHandler>.Raise(new MoneyChangePusHandler(gain));

        // 남은 수량이 0 이상 최대값 재설정
        if (currentSlot.runTimeIngredientData.ingredientQty > 0)
        {
            int max = currentSlot.runTimeIngredientData.ingredientQty;
            sellQty = Mathf.Min(sellQty, max);
            qtyInput.text = sellQty.ToString();
            UpdateTotal();
        }
    }
}
