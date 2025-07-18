using System;
using System.Collections;
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

    [Header("타이머 완료 시 보상 재료 목록")]
    [SerializeField] private List<IngredientReward> rewards = new List<IngredientReward>();

    // 보상용 데이터 구조체
    [Serializable]
    public class IngredientReward
    {
        [Tooltip("추가할 재료 이름")]
        public string ingredientName;
        [Tooltip("추가할 수량")]
        public int amount;
    }

    private DateTime _endTime;
    private bool _isCounting;

    private void Awake()
    {
        if (buyButton == null)
            buyButton = transform.Find("Buy/Soldout").GetComponent<Button>();
        if (timeText == null)
            timeText = transform.Find("Time/Value").GetComponent<TMP_Text>();

        buyButton.onClick.AddListener(OnBuyClicked);
        UpdateUIIdle();
    }

    private void OnBuyClicked()
    {
        SellItem();

        buyButton.interactable = false;
        _endTime = DateTime.Now.AddHours(durationHours);
        _isCounting = true;
        StartCoroutine(TimerCoroutine());
    }

    private IEnumerator TimerCoroutine()
    {
        while (_isCounting)
        {
            var rem = _endTime - DateTime.Now;
            if (rem.TotalSeconds <= 0)
            {
                _isCounting = false;
                timeText.text = "00:00:00";
                OnTimerComplete();
                yield break;
            }

            timeText.text = $"{rem.Hours:00}:{rem.Minutes:00}:{rem.Seconds:00}";
            yield return new WaitForSeconds(1f);
        }
    }

    private void OnTimerComplete()
    {
        abc();                        
        buyButton.interactable = true;
        UpdateUIIdle();
    }

    private void SellItem()
    {

        Debug.Log("Item sold!");
    }

    private void abc()
    {
        foreach (var reward in rewards)
        {
            InventoryManager.Instance.AddItem(reward.ingredientName, reward.amount);

            int nowQty = InventoryManager.Instance.GetItemQty(reward.ingredientName);
            Debug.Log($"[{reward.ingredientName}] +{reward.amount}, 현재 수량: {nowQty}");
        }
    }

    private void UpdateUIIdle()
    {
        timeText.text = "00:00:00";
    }
}
