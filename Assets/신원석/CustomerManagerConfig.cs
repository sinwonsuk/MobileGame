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

    public List<GameObject> GetGameObjects()
    {
        return GameObjects;
    }

    [field: SerializeField]
    List<GameObject> GameObjects { get; set; }
}