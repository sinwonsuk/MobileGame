using System.Collections.Generic;
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
    }
    ~FoodManager()
    {
        EventBus<FoodDecreaseHandler>.OnEvent -= DecreaseFood;
        EventBus<FoodIncreaseHandler>.OnEvent -= IncreaseFood;
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

    //public void GetExplanationFood(FoodDecreaseHandler foodAmountHandler)
    //{
    //    //for (int i = 0; i < conFig.GetFoods().Count; i++)
    //    //{
    //    //    if (conFig.GetFoods()[i].displayName == foodAmountHandler.foodname)
    //    //    {
    //    //        for (int j = 0; j < conFig.GetFoods()[i].Ingredients.Count; j++)
    //    //        {
    //    //            conFig.GetFoods()[i].Ingredients[j].qty -= foodAmountHandler.Setquantity;
    //    //        }
    //    //        return;
    //    //    }
    //    //}
    //}


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
