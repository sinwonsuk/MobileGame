using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Config/EnhanceFoodManagerConfig")]
public class EnhanceFoodManagerConfig : BaseScriptableObject
{
    public EnhanceFoodManagerConfig()
    {
        type = typeof(EnhanceFoodManagerConfig);
    }

    [field: SerializeField]
    List<GameObject>  enhanceFoodManagerUi;

    public List<GameObject> EnhanceFoodManagerUi
    {
        get => enhanceFoodManagerUi;
        set => enhanceFoodManagerUi = value;
    }


}

