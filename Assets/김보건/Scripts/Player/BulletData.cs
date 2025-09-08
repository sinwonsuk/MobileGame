using UnityEngine;

[CreateAssetMenu(fileName = "New BulletData", menuName = "Bullet/BulletData")]
public class BulletData : ScriptableObject
{
    public GameObject bulletPrefab;
    public float speed = 10f;
    public float damage = 5f;
    public bool isExplosive;
    public bool isPiercing;
}
