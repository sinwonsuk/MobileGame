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

    private bool _isCounting;
    private DateTime _endTime;

    private void Awake()
    {
        if (buyButton == null)
            buyButton = transform.Find("Buy/Soldout").GetComponent<Button>();
        if (timeText == null)
            timeText = transform.Find("Time/Value").GetComponent<TMP_Text>();

        buyButton.onClick.AddListener(OnBuyClicked);
        SetIdleUI();
    }

    private void OnBuyClicked()
    {
        SellItem();

        buyButton.interactable = false;
        _isCounting = true;
        _endTime = DateTime.Now.AddHours(durationHours);

        // 서비스에 타이머 등록 (UI가 꺼져도 계속 돌아감)
        ShopTimerService.Instance.RegisterTimer(
            this,
            _endTime,
            OnTimerComplete
        );
    }

    private void Update()
    {
        if (!_isCounting) return;

        // UI가 활성화되어 있는 동안만 남은 시간 갱신
        var rem = ShopTimerService.Instance.GetRemaining(this);
        if (rem > TimeSpan.Zero)
        {
            timeText.text = $"{rem.Hours:00}:{rem.Minutes:00}:{rem.Seconds:00}";
        }
        else
        {
            // (안전장치) Update 중에도 혹시 남은 시간이 0 이하라면 즉시 처리
            OnTimerComplete();
        }
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
