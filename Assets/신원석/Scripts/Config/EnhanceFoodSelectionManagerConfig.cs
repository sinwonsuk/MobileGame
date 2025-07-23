using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Config/EnhanceFoodSelectionManager")]
public class EnhanceFoodSelectionManagerConfig : BaseScriptableObject
{
    public EnhanceFoodSelectionManagerConfig()
    {
        type = typeof(FoodSelectionManagerConfig);
    }

    [field: SerializeField]
    List<GameObject> gameObjects { get; set; }

    public List<GameObject> GameObjects
    {
        get => gameObjects;
        set => gameObjects = value;
    }

}