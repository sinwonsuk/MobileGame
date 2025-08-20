using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // TMP 사용

public class InteriorShopButton : MonoBehaviour
{
    public InteriorData interiorData;
    public RunTimeInteriorData RuntimeInteriorData;
    public Button purchaseButton;
    public TMP_Text nameText;
    public TMP_Text ownedText;
    public TMP_Text priceText;
    private int money;

    void Start()
    {
        purchaseButton.onClick.AddListener(OnButtonClicked);


        if (nameText != null && interiorData != null)
            nameText.text = interiorData.interiorName;

        if (priceText != null && interiorData != null)
            priceText.text = $"{interiorData.BaseSalary:N0} G";

        if (ownedText != null)
            ownedText.gameObject.SetActive(false);

        money = interiorData.BaseSalary;
        hideButton();
    }

    private void OnButtonClicked()
    {

        if (RuntimeInteriorData.isOwned) return;
        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click, false);
        // ★ 현재 돈 확인 (예: userData.money가 현재 보유 금액)
        int currentMoney = BackendGameData.Instance.userData.gold;
        if (currentMoney < money)
        {
            PopupManager.Show("돈이 부족합니다");
            Debug.Log("구매 실패: 돈이 부족합니다.");
            return; // 구매 불가
        }

        EventBus<MoneyChangeMusHandler>.Raise(new MoneyChangeMusHandler(money));
        BackendGameData.Instance.AddReputation(5);
        InteriorManager.Instance.AcquireInterior(interiorData.interiorName);
        Debug.Log("구매");

        hideButton();
    }

    private void hideButton()
    {
        if (RuntimeInteriorData.isOwned)
        {
            purchaseButton.interactable = false;


            if (ownedText != null)
            {
                ownedText.text = "보유중";
                ownedText.gameObject.SetActive(true);
            }
        }
    }
}
