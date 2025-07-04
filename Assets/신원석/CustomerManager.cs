using UnityEngine;

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
        for (int i = 0; i < conFig.GetUiGameObjects().Count; i++)
        {
            GameObject.Instantiate(conFig.GetUiGameObjects()[i]);
        }
    }
}
