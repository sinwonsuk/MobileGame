using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inven/inven")]
public class RunTimeIngredientData : BaseScriptableObject
{
    public string ingredientName;
    public int ingredientQty;
}