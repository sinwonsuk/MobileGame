// Bullet2D.cs
using UnityEngine;

public class Bullet2D : MonoBehaviour
{
    [Header("설정")]
    [Tooltip("발사 후 자동 삭제 시간")]
    public float lifeTime = 5f;

    [Tooltip("타겟 태그")]
    public string targetTag = "a";

    int damage;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void SetDamage(int dmg)
    {
        damage = dmg;
    }

    // 트리거 충돌
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(targetTag))
        {
            // 보스 체력 스크립트 호출
            var bossHealth = other.GetComponent<BossHealth>();
            if (bossHealth != null)
                bossHealth.TakeDamage(damage);

            Destroy(gameObject);
            return;
        }

        // 벽 등 다른 것에 닿아도 제거
        if (!other.isTrigger)
            Destroy(gameObject);
    }
}
