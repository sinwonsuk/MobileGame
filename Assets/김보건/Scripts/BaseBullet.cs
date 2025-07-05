using UnityEngine;

public class BaseBullet : MonoBehaviour
{
    Vector2 direction;
    float speed;
    float damage;

    public void Initialize(Vector2 shootDirection, float bulletSpeed, float bulletDamage)
    {
        direction = shootDirection.normalized;
        speed = bulletSpeed;
        damage = bulletDamage;
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        EnemyBase enemy = other.GetComponent<EnemyBase>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Destroy(gameObject); 
        }
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
