using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.RuleTile.TilingRuleOutput;
using Transform = UnityEngine.Transform;

public class FoodUIManager : baseManager, IGameManager
{

    public FoodUIManager(FoodManagerConfig config)
    {
        conFig = config;
        EventBus<FoodDecreaseHandler>.OnEvent += DecreaseFood;
        EventBus<FoodIncreaseHandler>.OnEvent += IncreaseFood;
    }
    ~FoodUIManager()
    {
        EventBus<FoodDecreaseHandler>.OnEvent -= DecreaseFood;
        EventBus<FoodIncreaseHandler>.OnEvent -= IncreaseFood;
    }


    public FoodUIManager(BaseScriptableObject baseScriptableObject)
    {
        type = typeof(FoodUIManager);
        conFig = (FoodManagerConfig)baseScriptableObject;
    }

    public override void Init()
    {
        for (int i = 0; i < conFig.GetFoods().Count; i++)
        {
            EventBus<SlotSpawnHandler>.Raise(new SlotSpawnHandler(conFig.GetSlotUI(),conFig.GetFoods()[i]));
        }
    }

    public void DecreaseFood(FoodDecreaseHandler foodAmountHandler)
    {
        for (int i = 0; i < conFig.GetFoods().Count; i++)
        {
            if (conFig.GetFoods()[i].displayName == foodAmountHandler.foodname)
            {
                for (int j = 0; j < conFig.GetFoods()[i].Ingredients.Count; j++)
                {
                    conFig.GetFoods()[i].Ingredients[j].qty -= foodAmountHandler.Setquantity;
                }
                return;
            }
        }
    }

    public void IncreaseFood(FoodIncreaseHandler foodAmountHandler)
    {
        for (int i = 0; i < conFig.GetFoods().Count; i++)
        {
            if (conFig.GetFoods()[i].displayName == foodAmountHandler.foodname)
            {
                for (int j = 0; j < conFig.GetFoods()[i].Ingredients.Count; j++)
                {
                    conFig.GetFoods()[i].Ingredients[j].qty += foodAmountHandler.Setquantity;
                }
                return;
            }
        }
    }

    public override void Update()
    {
        
    }

    FoodManagerConfig conFig;

}
