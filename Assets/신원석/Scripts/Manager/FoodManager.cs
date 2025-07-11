using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.RuleTile.TilingRuleOutput;
using Transform = UnityEngine.Transform;

public class FoodManager : baseManager, IGameManager
{

    public FoodManager(FoodManagerConfig config)
    {
        conFig = config;
        EventBus<FoodDecreaseHandler>.OnEvent += DecreaseFood;
        EventBus<FoodIncreaseHandler>.OnEvent += IncreaseFood;
        EventBus<FoodQuantityChangedEvent>.OnEvent += GetFoodIngredientQty;
    }
    ~FoodManager()
    {
        EventBus<FoodDecreaseHandler>.OnEvent -= DecreaseFood;
        EventBus<FoodIncreaseHandler>.OnEvent -= IncreaseFood;
        EventBus<FoodQuantityChangedEvent>.OnEvent -= GetFoodIngredientQty;
    }


    public FoodManager(BaseScriptableObject baseScriptableObject)
    {
        type = typeof(FoodManager);
        conFig = (FoodManagerConfig)baseScriptableObject;
    }

    public override void Init()
    {
        for (int i = 0; i < conFig.GetFoods().Count; i++)
        {
            foodDic.Add(conFig.GetFoods()[i].displayName, conFig.GetFoods()[i]);
        }

        for (int i = 0; i < conFig.GetFoods().Count; i++)
        {
            EventBus<SlotSpawnHandler>.Raise(new SlotSpawnHandler(conFig.GetSlotUI(),conFig.GetFoods()[i]));
        }
    }

    public void GetFoodIngredientQty(FoodQuantityChangedEvent dddd)
    {
        foreach (var foodData in foodDic.Values)
        {
            foreach (var ingredient in foodData.Ingredients)
            {
                if (dddd.Inven.keyValuePairs.TryGetValue(ingredient.ingredientName, out var inven))
                {
                    ingredient.qty = dddd.Inven.keyValuePairs[ingredient.ingredientName];
                }
            }
        }
    }


    public void DecreaseFood(FoodDecreaseHandler foodAmountHandler)
    {
        if (foodDic.TryGetValue(foodAmountHandler.foodname, out var foodData))
        {
            for (int j = 0; j < foodData.Ingredients.Count; j++)
            {
                foodData.Ingredients[j].qty -= foodAmountHandler.Setquantity;
            }
            return;
        }   
    }

    public void IncreaseFood(FoodIncreaseHandler foodAmountHandler)
    {
        if (foodDic.TryGetValue(foodAmountHandler.foodname, out var foodData))
        {
            for (int j = 0; j < foodData.Ingredients.Count; j++)
            {
                foodData.Ingredients[j].qty += foodAmountHandler.Setquantity;
            }
        }      
    }

    public override void Update()
    {
        
    }

    Dictionary<string, FoodData> foodDic = new Dictionary<string, FoodData>();

    FoodManagerConfig conFig;

}
