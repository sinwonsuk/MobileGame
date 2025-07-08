using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Food/Ingredient")]
public class IngredientData : BaseScriptableObject
{
    public string Name;
    public string sprite;
    public int qty;

}