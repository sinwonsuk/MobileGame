using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.RuleTile.TilingRuleOutput;
using Transform = UnityEngine.Transform;

public class CustomerManager : baseManager, IGameManager
{

    public CustomerManager(CustomerManagerConfig config)
    {
        conFig = config;

        EventBus<CustomerSpawnHandler>.OnEvent += SpawnCustomer;
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

    public void SpawnCustomer(CustomerSpawnHandler customerSpawnHandler)
    {
        for (int i = 0; i < conFig.GetGameObjects().Count; i++)
        {
            GameObject.Instantiate(conFig.GetGameObjects()[i]);
        }
    }

    public override void Update()
    {
        if(Input.GetMouseButtonDown(0))  // Mouse button pressed
        {
            for (int i = 0; i < conFig.GetGameObjects().Count; i++)
            {
                GameObject.Instantiate(conFig.GetGameObjects()[i]);
            }
        }

    }

    CustomerManagerConfig conFig;

    List<Customer> customers = new List<Customer>();

}
