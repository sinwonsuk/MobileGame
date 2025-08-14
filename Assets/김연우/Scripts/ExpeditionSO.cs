using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Expedition_", menuName = "Game/Expedition/Static")]
public class ExpeditionSO : ScriptableObject
{
    [Header("고유 ID (런타임 SO와 동일)")]
    public string Indate;

    [Header("표시 이름")]
    public string displayName;

    [Header("소요 시간(시간)")]
    public float durationHours = 0.01f;

    [Header("보상 리스트")]
    public List<Reward> rewards = new();

    [Serializable]
    public class Reward
    {
        public RunTimeIngredientData ingredientData; // 네 프로젝트 타입 그대로 사용
        public int amount = 1;
    }
}
