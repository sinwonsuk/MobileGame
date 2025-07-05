using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.RuleTile.TilingRuleOutput;
using Transform = UnityEngine.Transform;

public class FoodSelectionManager : baseManager, IGameManager
{

    public FoodSelectionManager(FoodSelectionManagerConfig config)
    {
        conFig = config;

    }

    public FoodSelectionManager(BaseScriptableObject baseScriptableObject)
    {
        type = typeof(FoodSelectionManager);
        conFig = (FoodSelectionManagerConfig)baseScriptableObject;
    }

    public override void Init()
    {
        for (int i = 0; i < conFig.GetGameObjects().Count; i++)
        {
            GameObject.Instantiate(conFig.GetGameObjects()[i]);
        }
    }
    public override void Update()
    {
       
    }

    FoodSelectionManagerConfig conFig;

    List<Customer> customers = new List<Customer>();

}
