using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.RuleTile.TilingRuleOutput;
using Transform = UnityEngine.Transform;

public class CustomerManager : baseManager, IGameManager
{

    CustomerManagerConfig conFig;




    public CustomerManager(CustomerManagerConfig config)
    {

        conFig = config;

    }

    public CustomerManager(BaseScriptableObject baseScriptableObject)
    {
        type = typeof(CustomerManager);
        conFig = (CustomerManagerConfig)baseScriptableObject;
    }

    public override void Init()
    {
        for (int i = 0; i < conFig.GetGameObjects().Count; i++)
        {
            GameObject.Instantiate(conFig.GetGameObjects()[i]);
        }
    }

    [SerializeField]
    private Transform target;
    [SerializeField]
    private int enemyCount = 10;

}
