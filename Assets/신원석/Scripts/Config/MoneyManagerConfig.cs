using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Config/MoneyManager")]
public class MoneyManagerConfig : BaseScriptableObject
{
    public MoneyManagerConfig()
    {
        type = typeof(MoneyManagerConfig);
    }


    [field: SerializeField]
    GameObject moneyUI;

    public GameObject MoneyUI
    {
        get => moneyUI;
        set => moneyUI = value;
    }

}

