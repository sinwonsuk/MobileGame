// Bullet2D.cs
using UnityEngine;

public class Bullet2D : MonoBehaviour
{
    [Header("설정")]
    [Tooltip("발사 후 자동 삭제 시간")]
    public float lifeTime = 5f;

    [Tooltip("타겟 태그")]
    public string targetTag = "a";

    [SerializeField] private GameObject hitEffectPrefab;
    [SerializeField] private float hitEffectLifetime = 2f;

    [SerializeField] private string effectSortingLayer = "Effects"; // 프로젝트에서 캐릭터 레이어보다 위에 놓기
    [SerializeField] private int effectSortingOrder = 9999;


    double damage;
    bool hasHit = false;
    bool isPiercing = false;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void SetDamage(double dmg)
    {
        damage = dmg;
    }

    // 트리거 충돌
    void OnTriggerEnter2D(Collider2D other)
    {
        //if (other.CompareTag(targetTag))
        //{
        //    // 보스 체력 스크립트 호출
        //    var bossHealth = other.GetComponent<BossHealth>();
        //    if (bossHealth != null)
        //    {
        //        bossHealth.TakeDamage(damage);
        //        Destroy(gameObject);
        //        return;
        //    }

        //}
        if (hasHit && !isPiercing) return;

        var enemy = other.GetComponent<EnemyBase>();
        if (enemy != null)
        {
            hasHit = true;
            enemy.TakeDamage(damage);
            SpawnHitEffect(other);

            SoundManager.GetInstance().SfxPlay(SoundManager.sfx.PlayerAttack, false);

            if (!isPiercing)                 // 관통탄이 아니면 즉시 파괴
                Destroy(gameObject);
        }

        // 벽 등 다른 것에 닿아도 제거
        if (!other.isTrigger)
            Destroy(gameObject);
    }

    private void SpawnHitEffect(Collider2D target)
    {
        if (hitEffectPrefab == null) return;


        Vector3 pos = target.ClosestPoint(transform.position);

        //씬 최상위에 생성
        GameObject fx = Instantiate(hitEffectPrefab, pos, Quaternion.identity);

        // 파티클 렌더러들을 최상단 정렬로 강제
        var renderers = fx.GetComponentsInChildren<ParticleSystemRenderer>(true);
        foreach (var r in renderers)
        {
            r.sortingLayerName = effectSortingLayer; // ex) "Effects"
            r.sortingOrder = effectSortingOrder;     // ex) 9999
        }
        Destroy(fx, hitEffectLifetime);
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

}
