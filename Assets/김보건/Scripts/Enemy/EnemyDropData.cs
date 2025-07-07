using UnityEngine;


[CreateAssetMenu(menuName = "Enemy/DropTable")]
public class EnemyDropData : ScriptableObject
{
    public GameObject[] possibleDrops; // 드랍 가능한 아이템들
}
