using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Config/FoodManager")]
public class FoodManagerConfig : BaseScriptableObject
{
    public FoodManagerConfig()
    {
        type = typeof(FoodManagerConfig);
    }
     
    public List<FoodData> GetFoods()
    {
        return Foods;
    }
    public GameObject GetSlotUI()
    {
        return SlotUI;
    }
    [field: SerializeField]
    List<FoodData> Foods { get; set; }

    [field: SerializeField]
    GameObject SlotUI { get; set; }


}

