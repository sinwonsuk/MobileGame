using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Config/MenuManager")]
public class MenuManagerConfig : BaseScriptableObject
{
    public MenuManagerConfig()
    {
        type = typeof(MenuManagerConfig);
    }

    public List<GameObject> GetGameObjects()
    {
        return GameObjects;
    }

    [field: SerializeField]
    List<GameObject> GameObjects { get; set; }

    [field: SerializeField]
    ExplanationData explanationData { get; set; }


    public ExplanationData GetExplanationData
    {
       get => explanationData;
    }

}