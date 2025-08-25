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

    // 기존 좌/우 (±1)
    [SerializeField] private Button leftBtn;
    [SerializeField] private Button rightBtn;

    // 추가: 빠른 증감(±10)
    [SerializeField] private Button dec10Btn;
    [SerializeField] private Button inc10Btn;

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
        if (leftBtn != null) leftBtn.onClick.AddListener(() => OnStep(-1));
        if (rightBtn != null) rightBtn.onClick.AddListener(() => OnStep(+1));
        if (dec10Btn != null) dec10Btn.onClick.AddListener(() => OnStep(-10));
        if (inc10Btn != null) inc10Btn.onClick.AddListener(() => OnStep(+10));

        qtyInput.onValueChanged.AddListener(OnInputChanged);
        sellBtn.onClick.AddListener(OnSell);
    }

    void OnDisable()
    {
        if (leftBtn != null) leftBtn.onClick.RemoveAllListeners();
        if (rightBtn != null) rightBtn.onClick.RemoveAllListeners();
        if (dec10Btn != null) dec10Btn.onClick.RemoveAllListeners();
        if (inc10Btn != null) inc10Btn.onClick.RemoveAllListeners();

        qtyInput.onValueChanged.RemoveAllListeners();
        sellBtn.onClick.RemoveAllListeners();
    }

    private void ShowPanel(InventorySlot slot)
    {
        currentSlot = slot;
        sellQty = 1; // 초기값 (원하면 0으로 바꿔도 됨)

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

    // 공통 증감 핸들러: 랩어라운드 + 클램프
    private void OnStep(int delta)
    {
        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click, false);
        if (currentSlot == null) return;

        int max = Mathf.Max(0, currentSlot.runTimeIngredientData.ingredientQty);

        if (delta < 0)
        {
            // 감소: 0에서 뒤(-) 누르면 최대값으로 랩어라운드
            if (sellQty <= 0)
                sellQty = max;
            else
                sellQty = Mathf.Max(0, sellQty + delta); // -10 같은 경우 0 미만이면 0으로
        }
        else if (delta > 0)
        {
            // 증가: 최대에서 앞(+) 누르면 0으로 랩어라운드
            if (sellQty >= max)
                sellQty = 0;
            else
                sellQty = Mathf.Min(max, sellQty + delta); // +10 같은 경우 최대 넘기면 최대로
        }

        qtyInput.SetTextWithoutNotify(sellQty.ToString());
        UpdateTotal();
    }

    private void OnInputChanged(string s)
    {
        if (currentSlot == null) return;

        if (int.TryParse(s, out int v))
        {
            int max = Mathf.Max(0, currentSlot.runTimeIngredientData.ingredientQty);
            // 입력은 0~max 범위로 클램프
            sellQty = Mathf.Clamp(v, 0, max);
            qtyInput.SetTextWithoutNotify(sellQty.ToString());
            UpdateTotal();
        }
        else
        {
            // 숫자 아님 → 이전 값 유지
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
        // 필요하면 여기서 sellBtn.interactable = sellQty > 0; 로 0개 판매 방지도 가능
    }

    private void OnSell()
    {
        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click, false);
        if (currentSlot == null) return;

        // 0개면 아무 일도 안 하게 하려면 가드 추가:
        // if (sellQty <= 0) return;

        // 1) 인벤토리에서 수량 차감
        InventoryManager.Instance.DecreaseQty(currentSlot.ingredient.indate, sellQty);

        // 2) 돈 획득 이벤트
        int gain = sellQty * currentSlot.ingredient.ingredientPrice;
        EventBus<MoneyChangePusHandler>.Raise(new MoneyChangePusHandler(gain));

        // 3) 남은 수량이 있으면 입력값 재조정
        int max = currentSlot.runTimeIngredientData.ingredientQty;
        if (max > 0)
        {
            // 현재 선택이 남은 수량 초과면 맞춰주기
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
