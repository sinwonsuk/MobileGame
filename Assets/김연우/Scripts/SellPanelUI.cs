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
        InventorySlotUI.OnSlotClicked += ShowPanel;
    }

    void OnDestroy()
    {
        InventorySlotUI.OnSlotClicked -= ShowPanel;
    }

    void OnEnable()
    {
        leftBtn.onClick.AddListener(OnLeft);
        rightBtn.onClick.AddListener(OnRight);
        qtyInput.onValueChanged.AddListener(OnInputChanged);
        sellBtn.onClick.AddListener(OnSell);
    }

    void OnDisable()
    {
        leftBtn.onClick.RemoveAllListeners();
        rightBtn.onClick.RemoveAllListeners();
        qtyInput.onValueChanged.RemoveAllListeners();
        sellBtn.onClick.RemoveAllListeners();
    }

    private void ShowPanel(InventorySlot slot)
    {
        currentSlot = slot;
        sellQty = 1;

        // UI 세팅
        previewImage.sprite = Resources.Load<Sprite>(slot.ingredient.ingredientSprite);
        nameText.text = slot.ingredient.ingredientName;
        priceText.text = slot.ingredient.ingredientPrice.ToString();

        // 초기 입력값 설정 (콜백 없이)
        qtyInput.SetTextWithoutNotify(sellQty.ToString());
        UpdateTotal();

        // 판매 버튼 활성화
        sellBtn.interactable = true;

        gameObject.SetActive(true);
    }

    private void OnLeft()
    {
        if (sellQty > 1)
        {
            sellQty--;
            qtyInput.SetTextWithoutNotify(sellQty.ToString());
            UpdateTotal();
        }
    }

    private void OnRight()
    {
        if (currentSlot == null) return;

        int max = currentSlot.runTimeIngredientData.ingredientQty;
        if (sellQty < max)
        {
            sellQty++;
            qtyInput.SetTextWithoutNotify(sellQty.ToString());
            UpdateTotal();
        }
    }

    private void OnInputChanged(string s)
    {
        // 슬롯이 없으면 아무 것도 없음
        if (currentSlot == null)
            return;

        if (int.TryParse(s, out int v))
        {
            int max = currentSlot.runTimeIngredientData.ingredientQty;
            sellQty = Mathf.Clamp(v, 1, max);
            qtyInput.SetTextWithoutNotify(sellQty.ToString());
            UpdateTotal();
        }
        else
        {
            qtyInput.SetTextWithoutNotify(sellQty.ToString());
        }
    }

    private void UpdateTotal()
    {
        if (currentSlot == null)
        {
            totalText.text = string.Empty;
            return;
        }

        int total = sellQty * currentSlot.ingredient.ingredientPrice;
        totalText.text = total.ToString();
    }

    private void OnSell()
    {
        if (currentSlot == null) return;

        // 1) 인벤토리에서 수량 차감
        InventoryManager.Instance.DecreaseQty(currentSlot.ingredient.indate, sellQty);

        // 2) 돈 획득 이벤트
        int gain = sellQty * currentSlot.ingredient.ingredientPrice;
        EventBus<MoneyChangePusHandler>.Raise(new MoneyChangePusHandler(gain));

        // 3) 남은 수량이 있으면 입력값 재조정
        if (currentSlot.runTimeIngredientData.ingredientQty > 0)
        {
            int max = currentSlot.runTimeIngredientData.ingredientQty;
            sellQty = Mathf.Min(sellQty, max);
            qtyInput.SetTextWithoutNotify(sellQty.ToString());
            UpdateTotal();
        }

        // 4) 필드만 초기화 (패널은 유지)
        ClearFields();
    }

    private void ClearFields()
    {
        currentSlot = null;
        sellQty = 0;

        previewImage.sprite = null;
        nameText.text = string.Empty;
        priceText.text = string.Empty;
        totalText.text = string.Empty;

        // 콜백 없이 입력만 지우기
        qtyInput.SetTextWithoutNotify(string.Empty);

        // 버튼 비활성화
        sellBtn.interactable = false;
    }
}
