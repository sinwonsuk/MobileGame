using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Food/FoodData")]
public class FoodData : BaseScriptableObject
{
    public string indate;
    public string displayName;      // 화면에 보일 이름
    public string foodSprite;       // 사진
    public int price;            // 가격
    public int reputation;       // 명성도
    public int Getreputation;       // 명성도
    public bool isUnlock;          // 해금 여부     
    public float cookingTime; // 대기 시간 
    public int Level;

    public List<IngredientData> Ingredients; // 재료 목록
    public List<EnhanceStepData> enhanceSteps; // 강화 단계 목록
}

[System.Serializable]
public class EnhanceStepData
{
    public string indate;
    public string foodIndate;
    public int step;
    public float successRate;
    public List<EnhanceMaterialData> ingredients;
    public int cost;
}

[System.Serializable]
public class EnhanceMaterialData
{
    public string indate;
    public string name;
    public int quantity;
}