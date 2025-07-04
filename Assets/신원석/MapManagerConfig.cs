
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Config/MapManager")]


public class MapManagerConfig : BaseScriptableObject
{
    public MapManagerConfig()
    {
        type = typeof(MapManagerConfig);
    }

    public List<GameObject> GetMapGameObjects()
    {
        return mapGameObjects;
    }

    [field: SerializeField]
    List<GameObject> mapGameObjects { get; set; }
}

