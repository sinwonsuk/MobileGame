using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Config/CoookManager")]
public class CookManagerConfig : BaseScriptableObject
{
    public CookManagerConfig()
    {
        type = typeof(CookManagerConfig);
    }


    [field: SerializeField]
    List<FoodData> foods { get; set; }

    public List<FoodData> Foods
    {
        get => foods;
        set => foods = value;
    }

    [field: SerializeField]
    List<GameObject> gameObjects { get; set; }

    public List<GameObject> GameObjects
    {
        get => gameObjects;
        set => gameObjects = value;
    }



}

