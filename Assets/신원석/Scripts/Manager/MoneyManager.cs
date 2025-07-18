using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.STP;



public class MoneyManager : baseManager, IGameManager
{
    public MoneyManager(MoneyManagerConfig config)
    {
        conFig = config;

        EventBus<MoneyChangeMusHandler>.OnEvent += UseMoney;
        EventBus<MoneyChangePusHandler>.OnEvent += GainMoney;

    }
    ~MoneyManager()
    {
        EventBus<MoneyChangeMusHandler>.OnEvent -= UseMoney;
        EventBus<MoneyChangePusHandler>.OnEvent -= GainMoney;
    }
    public MoneyManager(BaseScriptableObject baseScriptableObject)
    {
        type = typeof(CookManager);
        conFig = (MoneyManagerConfig)baseScriptableObject;
    }

    public override void Init()
    {
        Transform transform = GameObject.FindGameObjectWithTag("Canvas").transform;
        GameObject obj = GameObject.Instantiate(conFig.MoneyUI, transform);

        Money money = conFig.MoneyUI.GetComponent<Money>();

        string adsd = money.MoneyText.text;

        moneyObject = obj;
    }


    public void UseMoney(MoneyChangeMusHandler moneyChangeHandler)
    {
        Money money = moneyObject.GetComponent<Money>();

        int intmoney = int.Parse(money.MoneyText.text);

        if(intmoney < moneyChangeHandler.money)
        {
            Debug.Log("µ· ºÎÁ·ÇÔ");
            return;
        }
        else
        {
            intmoney -= moneyChangeHandler.money;
            money.MoneyText.text = intmoney.ToString();
        }

    }

    public void GainMoney(MoneyChangePusHandler moneyChangeHandler)
    {
        Money money = moneyObject.GetComponent<Money>();
        int intmoney = int.Parse(money.MoneyText.text);

        intmoney += moneyChangeHandler.money;
        money.MoneyText.text = intmoney.ToString();
    }


    public override void Update()
    {

    }

    MoneyManagerConfig conFig;

    GameObject moneyObject;
}
