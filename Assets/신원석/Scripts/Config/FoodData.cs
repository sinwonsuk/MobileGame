using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Food/FoodData")]
public class FoodData : BaseScriptableObject
{
    public string displayName;      // 화면에 보일 이름
    public string foodSprite;       // 사진
    public int price;            // 가격
    public bool isUnlock;          // 해금 여부 
    public string explanation;
    //public List<Ingredient> ingredients;

    public List<IngredientData> Ingredients;


    //[System.Serializable]
    //public class Ingredient
    //{
    //    public string name;
    //    public string sprite;
    //    public int qty;

    //}
}