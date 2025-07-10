using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Config/TableManager")]
public class TableManagerConfig : BaseScriptableObject
{
    public TableManagerConfig()
    {
        type = typeof(TableManagerConfig);
    }

    public List<GameObject> GetGameObjects()
    {
        return GameObjects;
    }

    [field: SerializeField]
    List<GameObject> GameObjects { get; set; }
}