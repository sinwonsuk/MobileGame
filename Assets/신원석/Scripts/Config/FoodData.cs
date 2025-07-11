using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Food/FoodData")]
public class FoodData : BaseScriptableObject
{
    public string inDate;
    public string displayName;      // ȭ�鿡 ���� �̸�
    public string foodSprite;       // ����
    public int price;            // ����
    public bool isUnlock;          // �ر� ���� 
    public float cookingTime; // ��� �ð� 

    public List<IngredientData> Ingredients;

}