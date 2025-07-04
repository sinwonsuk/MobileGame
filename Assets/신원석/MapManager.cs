using UnityEngine;

public class MapManager : baseManager, IGameManager
{

    MapManagerConfig conFig;

    public MapManager(MapManagerConfig config)
    {
        conFig = config;
    }

    public MapManager(BaseScriptableObject baseScriptableObject)
    {
        type = typeof(UIManager);
        conFig = (MapManagerConfig)baseScriptableObject;
    }

    public override void Init()
    {
        GameObject.Instantiate(conFig);
    }
}
