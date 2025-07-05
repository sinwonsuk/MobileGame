using System;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager : baseManager,IGameManager
{
    UIManagerConfig conFig;

    public UIManager(UIManagerConfig config)
    {
        conFig = config;
    }

    public UIManager(BaseScriptableObject baseScriptableObject)
    {
        type = typeof(UIManager);
        conFig = (UIManagerConfig)baseScriptableObject;
    }

    public override void Init()
    {
        for (int i = 0; i < conFig.GetUiGameObjects().Count; i++)
        {
          GameObject.Instantiate(conFig.GetUiGameObjects()[i]);
        }
    }
    public override void Update()
    {
        // UIManager specific update logic can be added here
    }
}
