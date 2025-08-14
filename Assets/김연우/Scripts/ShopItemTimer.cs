using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemTimer : MonoBehaviour
{
    [Header("UI 참조")]
    [SerializeField] private Button buyButton;
    [SerializeField] private TMP_Text timeText;

    [Header("타이머 설정")]
    [SerializeField] private float durationHours = 2f;

    [Header("타이머 완료 시 보상 목록")]
    [SerializeField] private List<IngredientReward> rewards = new List<IngredientReward>();

    [Serializable]
    public class IngredientReward
    {
        public RunTimeIngredientData ingredientData;
        public int amount;
    }

    [SerializeField] private string itemKey; // 인게임 세션 구분용 키 (DB/저장 없이 세션만 유지)

    private bool _isCounting;
    private DateTime _endTime;

    private void Awake()
    {
        if (buyButton == null)
            buyButton = transform.Find("Buy/Soldout").GetComponent<Button>();
        if (timeText == null)
            timeText = transform.Find("Time/Value").GetComponent<TMP_Text>();

        buyButton.onClick.AddListener(OnBuyClicked);

        // 세션 내에서만 유지 → 게임 재시작 시 초기화
        SetIdleUI();
    }

    private void Update()
    {
        if (!_isCounting) return;

        // UI가 활성화되어 있는 동안만 남은 시간 갱신
        var remaining = _endTime - DateTime.UtcNow;
        if (remaining > TimeSpan.Zero)
        {
            timeText.text = $"{remaining.Hours:00}:{remaining.Minutes:00}:{remaining.Seconds:00}";
        }
        else
        {
            // 남은 시간이 0 이하라면 즉시 처리
            OnTimerComplete();
        }
    }

    private void OnBuyClicked()
    {
        SellItem();

        buyButton.interactable = false;
        _isCounting = true;
        _endTime = DateTime.UtcNow.AddHours(durationHours);

        // 서비스에 타이머 등록 (선택적으로만 사용)
        ShopTimerService.Instance.RegisterTimer(this, _endTime, OnTimerComplete);
    }

    private void OnTimerComplete()
    {
        if (!_isCounting) return;
        _isCounting = false;

        timeText.text = "00:00:00";
        buyButton.interactable = true;
        GiveRewards();
    }

    private void SellItem()
    {
        Debug.Log("Item sold!");
    }

    private void GiveRewards()
    {
        foreach (var reward in rewards)
        {
            var data = reward.ingredientData;
            InventoryManager.Instance.AddItem(data.indate, reward.amount);

            int nowQty = InventoryManager.Instance.GetItemQty(data.indate);
            Debug.Log($"[{data.ingredientName}] +{reward.amount}, 현재 수량: {nowQty}");
        }
    }

    private void SetIdleUI()
    {
        timeText.text = "00:00:00";
        buyButton.interactable = true;
    }
}
