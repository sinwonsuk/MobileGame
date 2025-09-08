using UnityEngine;

public class BaseBullet : MonoBehaviour
{
    Vector2 direction;
    float speed;
    float damage;
    bool hasHit = false;
    bool isPiercing = false;

    [SerializeField] private GameObject hitEffectPrefab;
    [SerializeField] private float hitEffectLifetime = 2f;

    private const float dungeonMaxX = -5.3f;

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

        if (transform.position.x > dungeonMaxX)
        {
            Destroy(gameObject);
            return;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit && !isPiercing) return;

        EnemyBase enemy = other.GetComponent<EnemyBase>();
        if (enemy != null)
        {
            hasHit = true;
            enemy.TakeDamage(damage);

            SpawnHitEffect(other);

            SoundManager.GetInstance().SfxPlay(SoundManager.sfx.PlayerAttack, false);

            if (!isPiercing)                 // 관통탄이 아니면 즉시 파괴
                Destroy(gameObject);
        }

        if (other.CompareTag("Map"))
        {
            Destroy(gameObject);
        }
    }

    private void SpawnHitEffect(Collider2D target)
    {
        if (hitEffectPrefab == null) return;

        Vector3 pos = target.ClosestPoint(transform.position);

        // 씬 최상위에 생성
        GameObject fx = Instantiate(hitEffectPrefab, pos, Quaternion.identity);

        // 파티클 렌더러 최상단 정렬
        var renderers = fx.GetComponentsInChildren<ParticleSystemRenderer>(true);
        foreach (var r in renderers)
        {
            r.sortingLayerName = "Effects"; // 프로젝트에서 만든 최상단 레이어
            r.sortingOrder = 9999;
        }

        Destroy(fx, hitEffectLifetime);
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
