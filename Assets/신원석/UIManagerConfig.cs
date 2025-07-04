using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Config/UIManager")]
public class UIManagerConfig : BaseScriptableObject
{
    public UIManagerConfig()
    {
        type = typeof(UIManagerConfig);
    }

    public List<GameObject> GetUiGameObjects()
    {
        return UiGameObjects;
    }

    [field:SerializeField]
    List<GameObject> UiGameObjects { get; set; }
}

