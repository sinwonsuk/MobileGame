using UnityEngine;

public class BaseBullet : MonoBehaviour
{
    Vector2 direction;
    float speed;

    public void Initialize(Vector2 shootDirection, float bulletSpeed)
    {
        direction = shootDirection.normalized;
        speed = bulletSpeed;
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
