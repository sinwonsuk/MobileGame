using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/EnemyData")]
public class EnemyData : ScriptableObject
{
    public GameObject enemyPrefab;
    public float maxHp;
    public float moveSpeed;
    public string description;
}
