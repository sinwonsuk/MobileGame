using UnityEngine;

public class BaseBullet : MonoBehaviour
{
    Vector2 direction;
    float speed;
    float damage;
    bool hasHit = false;
    bool isPiercing = false;

    public void Initialize(Vector2 shootDirection, float bulletSpeed, float bulletDamage)
    {
        direction = shootDirection.normalized;
        speed = bulletSpeed;
        damage = bulletDamage;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit && !isPiercing) return;

        EnemyBase enemy = other.GetComponent<EnemyBase>();
        if (enemy != null)
        {
            hasHit = true;
            enemy.TakeDamage(damage);

            if (!isPiercing)                 // 관통탄이 아니면 즉시 파괴
                Destroy(gameObject);
        }
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
