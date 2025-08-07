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
        EventBus<EnhanceFoodSlotsSpawnHandler>.OnEvent += CreateEnhanceFoodSlot;
        EventBus<FoodSlotsSpawnHandler>.OnEvent += CreateFoodSlot;
        EventBus<EnhanceFoodSlotsDeleteHandler>.OnEvent += DeleteEnhanceFoodSlot;
        EventBus<FoodSlotsDeleteHandler>.OnEvent += DeleteFoodSlot;

        EventBus<EnhanceFoodDecreaseHandler>.OnEvent += DecreaseEnhanceFood;
    }
    ~FoodManager()
    {
        EventBus<FoodDecreaseHandler>.OnEvent -= DecreaseFood;
        EventBus<FoodIncreaseHandler>.OnEvent -= IncreaseFood;

        EventBus<FoodSlotsSpawnHandler>.OnEvent -= CreateFoodSlot;
        EventBus<EnhanceFoodSlotsSpawnHandler>.OnEvent -= CreateEnhanceFoodSlot;
        EventBus<EnhanceFoodSlotsDeleteHandler>.OnEvent -= DeleteEnhanceFoodSlot;
        EventBus<EnhanceFoodDecreaseHandler>.OnEvent -= DecreaseEnhanceFood;
        EventBus<FoodSlotsDeleteHandler>.OnEvent -= DeleteFoodSlot;
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

        //for (int i = 0; i < conFig.GetFoods().Count; i++)
        //{
        //    EventBus<SlotSpawnHandler>.Raise(new SlotSpawnHandler(conFig.GetSlotUI(),conFig.GetFoods()[i]));
        //}

    }

    public void CreateFoodSlot(FoodSlotsSpawnHandler slotSpawnsHandler)
    {
        for (int i = 0; i < conFig.GetFoods().Count; i++)
        {
            EventBus<SlotSpawnHandler>.Raise(new SlotSpawnHandler(conFig.GetSlotUI(), conFig.GetFoods()[i]));
        }
    }

    public void DeleteFoodSlot(FoodSlotDeleteHandler enhanceFoodData)
    {
        EventBus<FoodSlotDeleteHandler>.Raise(new FoodSlotDeleteHandler());
    }


    public void CreateEnhanceFoodSlot(EnhanceFoodSlotsSpawnHandler enhanceFoodData)
    {
        for (int i = 0; i < conFig.GetFoods().Count; i++)
        {
            EventBus<EnhanceFoodSlotSpawnHandler>.Raise(new EnhanceFoodSlotSpawnHandler(conFig.GetEnhanceSlotUI(), conFig.GetFoods()[i]));
        }
    }

    public void DeleteEnhanceFoodSlot(EnhanceFoodSlotsDeleteHandler enhanceFoodData)
    {
         EventBus<EnhanceFoodSlotDeleteHandler>.Raise(new EnhanceFoodSlotDeleteHandler());
    }
    public void DeleteFoodSlot(FoodSlotsDeleteHandler enhanceFoodData)
    {
        EventBus<FoodSlotDeleteHandler>.Raise(new FoodSlotDeleteHandler());
    }

    public void SetFoodData(FoodData foodData)
    {
        if (foodDic.ContainsKey(foodData.displayName))
        {
            foodDic[foodData.displayName] = foodData;
        }
        else
        {
            foodDic.Add(foodData.displayName, foodData);
        }
    }

    public void DecreaseFood(FoodDecreaseHandler foodAmountHandler)
    {
        if (foodDic.TryGetValue(foodAmountHandler.foodname, out var foodData))
        {
            for (int j = 0; j < foodData.Ingredients.Count; j++)
            {
                InventoryManager.Instance.DecreaseQty(foodData.Ingredients[j].indate, foodAmountHandler.Setquantity);
            }
            return;
        }   
    }

    public void DecreaseEnhanceFood(EnhanceFoodDecreaseHandler foodAmountHandler)
    {
        if (foodDic.TryGetValue(foodAmountHandler.foodName, out var foodData))
        {
            for (int j = 0; j < foodData.Ingredients.Count; j++)
            {
                if (foodData.Ingredients[j].ingredientName == foodAmountHandler.ingredientName)
                {
                    InventoryManager.Instance.DecreaseQty(foodData.Ingredients[j].indate, foodAmountHandler.Setquantity);
                    return;
                }
            }           
        }
    }


    public void IncreaseFood(FoodIncreaseHandler foodAmountHandler)
    {
        if (foodDic.TryGetValue(foodAmountHandler.foodname, out var foodData))
        {
            for (int j = 0; j < foodData.Ingredients.Count; j++)
            {
                //foodData.Ingredients[j].qty += foodAmountHandler.Setquantity;
            }
        }      
    }

    public override void Update()
    {
        
    }

    Dictionary<string, FoodData> foodDic = new Dictionary<string, FoodData>();

    FoodManagerConfig conFig;

}
