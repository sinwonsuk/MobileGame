using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Food/Ingredient")]
public class IngredientData : BaseScriptableObject
{
    public string inDate;
    public string ingredientName;
    public string ingredientSprite;
    public int ingredientPrice;
    public int qty;

}