using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inven/inven")]
public class RunTimeIngredientData : BaseScriptableObject
{
    public string indate;
    public string ingredientName;
    public int ingredientQty;

	[NonSerialized]
	public bool isDirty = false; // 변경 여부 체크용
}