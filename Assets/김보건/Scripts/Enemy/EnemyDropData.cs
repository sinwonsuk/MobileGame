using UnityEngine;


[CreateAssetMenu(menuName = "Enemy/DropTable")]
public class EnemyDropData : ScriptableObject
{
    [Header("ÀÏ¹Ý µå¶ø")]
    public GameObject[] commonDrops;

    [Header("Èñ±Í µå¶ø")]
    public GameObject[] rareDrops;

    [Header("ÃÑ µå¶ø")]
    public int dropCount = 1;

    [Header("Èñ±Í µå¶ø È®·ü")]
    [Range(0f, 1f)] public float rareChance = 0.02f;

}
