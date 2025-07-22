using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStatData", menuName = "Stat/PlayerStatData")]
public class PlayerStatData : ScriptableObject
{
    public float attackPower = 10f;
    public float critChance = 0f;
    public float autoAttackInterval = 1f;
    public float autoAttackDamage = 5f;
    public float critDamageMultiplier = 1.5f;
}
