using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Customer/CustomerTypeData")]
public class CustomerTypeData : ScriptableObject
{
    public string customerName;              // 예: "햄버거 손님"
    public Sprite sprite;                    // 외형
    public List<FoodData> possibleFoods;     // 이 손님이 먹을 수 있는 음식 리스트
    public float priceMultiplier = 1.0f;     // 가격 배수 등
    // ... 추가 속성
}