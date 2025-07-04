using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "Clicker/Player Stats")]
public class PlayerStatsSO : ScriptableObject
{
    [Header("Base Stats")]
    public float attackPower = 10f;
    public float critChance = 0.05f;
    public float autoAttackInterval = 1f;
    public float autoAttackDamage = 5f;
    public float critDamageMultiplier = 1.5f;
}