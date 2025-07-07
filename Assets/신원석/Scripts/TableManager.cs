using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.Rendering.DebugUI;
using static UnityEngine.RuleTile.TilingRuleOutput;
using Transform = UnityEngine.Transform;

public class TableManager : baseManager, IGameManager
{

    public TableManager(TableManagerConfig config)
    {
        conFig = config;

        //EventBus<CustomerSpawnHandler>.OnEvent += SpawnCustomer;
    }

    public TableManager(BaseScriptableObject baseScriptableObject)
    {
        type = typeof(TableManager);
        conFig = (TableManagerConfig)baseScriptableObject;
    }

    public override void Init()
    {
        for (int i = 0; i < conFig.GetGameObjects().Count; i++)
        {
            GameObject obj = GameObject.Instantiate(conFig.GetGameObjects()[i]);

            CustomerTable table = obj.GetComponent<CustomerTable>();

            tables.Add(table);
        }
    }

    public override void Update()
    {

    }

    TableManagerConfig conFig;

    List<CustomerTable> tables = new List<CustomerTable>();

    public void CheckSitTable()
    {
        int index = Random.Range(0, tables.Count);

        if(tables[index].isActiveAndEnabled ==true)
        {

        }
        else
        {

        }           
    }


}