using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Food/FoodData")]
public class FoodData : BaseScriptableObject
{
    public string displayName;      // ȭ�鿡 ���� �̸�
    public string foodSprite;       // ����
    public int price;            // ����
    public bool isUnlock;          // �ر� ���� 
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