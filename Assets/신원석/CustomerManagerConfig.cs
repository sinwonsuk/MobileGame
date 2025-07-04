using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Config/CustomerManager")]
public class CustomerManagerConfig : BaseScriptableObject
{
    public CustomerManagerConfig()
    {
        type = typeof(UIManagerConfig);
    }

    public List<GameObject> GetUiGameObjects()
    {
        return UiGameObjects;
    }

    [field: SerializeField]
    List<GameObject> UiGameObjects { get; set; }
}