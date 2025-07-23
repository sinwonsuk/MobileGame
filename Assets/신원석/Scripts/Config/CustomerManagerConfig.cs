using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Config/CustomerManager")]
public class CustomerManagerConfig : BaseScriptableObject
{
    public CustomerManagerConfig()
    {
        type = typeof(CustomerManagerConfig);
    }

    public List<GameObject> Customers
    {
        get => gameObjects;
        set => gameObjects = value;
    }

    [field: SerializeField]
    List<GameObject> gameObjects { get; set; }
}