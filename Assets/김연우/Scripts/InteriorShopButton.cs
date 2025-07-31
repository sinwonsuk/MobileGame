using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InteriorShopButton : MonoBehaviour
{
    public InteriorData interiorData;
    public RunTimeInteriorData RuntimeInteriorData;
    public Button purchaseButton;
    private int money;
    void Start()
    {
        purchaseButton.onClick.AddListener(OnButtonClicked);
        money = interiorData.BaseSalary;
        hideButton();
    }

    private void OnButtonClicked()
    {
        if (RuntimeInteriorData.isOwned == false)
        {
            EventBus<MoneyChangeMusHandler>.Raise(new MoneyChangeMusHandler(money));
            // 이 줄로 변경!
            InteriorManager.Instance.AcquireInterior(interiorData.interiorName);
            Debug.Log("구매");
        }
        hideButton();
    }
    private void hideButton()
    {
        if (RuntimeInteriorData.isOwned == true)
        {
            purchaseButton.interactable = false;
        }
    }
}
