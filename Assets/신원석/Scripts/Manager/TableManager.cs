using BackEnd.Tcp;
using System.Collections.Generic;
using System.Linq;
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

        EventBus<SitTableHandler>.OnEvent += CheckSitTable;
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

    List<int> tablesIndex = new List<int>();
    public void SitTableCheck()
    {
        for (int i = 0; i < tables.Count; i++)
        {
            if (tables[i].IsSittingAtTable ==false) // 앉을수 있는 거 돌면서 인덱스 저장 
                tablesIndex.Add(i);
        }
    }

    public void CheckSitTable(SitTableHandler sitTableHandler)
    {
       SitTableCheck();

        if (tablesIndex.Count == 0)
            return;

       int randomValue = Random.Range(0, tablesIndex.Count);
       int tableIndex = tablesIndex[randomValue];

       sitTableHandler.customer.Target = tables[tableIndex].TargetTransform;
       tables[tableIndex].IsSittingAtTable = true;
       sitTableHandler.customer.customerTable = tables[tableIndex];
       tablesIndex.Clear();
    }
}