using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Config/FoodSelectionManager")]
public class FoodSelectionManagerConfig : BaseScriptableObject
{
    public FoodSelectionManagerConfig()
    {
        type = typeof(FoodSelectionManagerConfig);
    }

    public List<GameObject> GetGameObjects()
    {
        return GameObjects;
    }

    [field: SerializeField]
    List<GameObject> GameObjects { get; set; }
}