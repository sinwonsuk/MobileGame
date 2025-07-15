// == StaffBehavior.cs ==
using UnityEngine;
using System.Collections;

public class StaffBehavior : MonoBehaviour
{
    StaffStatsSO data;

    [SerializeField] Animator animator;

    // 런타임에 계산된 실제 스탯
    int currentAttackPower;
    float currentAttackSpeed;

    Transform boss;

    [Header("발사 설정")]
    public GameObject bulletPrefab;
    public Transform firePoint;

    [Header("타겟 필터링")]
    public string bossTag = "a";
    public string bossLayerName = "Boss";

    [SerializeField]
    float detectRange = 10f;

    public void Init(StaffStatsSO stats)
    {
        data = stats;
        data.level = 1;                       // 최초 레벨 1
        RecalculateStats();
        StartCoroutine(FindAndShoot());
    }

    public void LevelUp()
    {
        data.level++;
        RecalculateStats();
        Debug.Log($"{data.displayName} leveled to {data.level}: " +
                  $"Power={currentAttackPower}, Speed={currentAttackSpeed}");
    }

    void RecalculateStats()
    {
        // 레벨에 따라 스탯 재계산
        currentAttackPower = data.attack_Power
                           + data.attack_PowerPerLevel * (data.level - 1);
        currentAttackSpeed = data.attack_Speed
                           + data.attack_SpeedPerLevel * (data.level - 1);
    }

    private IEnumerator FindAndShoot()
    {
        float interval = 1f / Mathf.Max(currentAttackSpeed, 0.01f);

        while (true)
        {
            GameObject[] targets = GameObject.FindGameObjectsWithTag(bossTag);
            float minDist = Mathf.Infinity;
            Transform nearest = null;

            foreach (var t in targets)
            {
                if (t.layer != LayerMask.NameToLayer(bossLayerName)) continue;

                float dist = Vector2.Distance(firePoint.position, t.transform.position);
                if (dist < minDist && dist <= detectRange)
                {
                    minDist = dist;
                    nearest = t.transform;
                }
            }

            boss = nearest;

            if (boss != null)
                Shoot2D();

            yield return new WaitForSeconds(interval);
        }
    }

    private void Shoot2D()
    {
        if (boss == null || bulletPrefab == null) return;

        if (animator != null)
            animator.SetTrigger("AttackTrigger");

        //Vector2 dir = (boss.position - firePoint.position).normalized;

        //float offset = 0.3f; // 발사 위치 간격

        //Vector3 leftFirePos = firePoint.position + firePoint.right * -offset;
        //Vector3 rightFirePos = firePoint.position + firePoint.right * offset;

        //FireBullet(leftFirePos, dir);
        //FireBullet(rightFirePos, dir);
    }

    private void FireBullet(Vector3 position, Vector2 direction)
    {
        var go = Instantiate(bulletPrefab, position, Quaternion.identity);
        go.transform.right = direction;

        var bullet = go.GetComponent<Bullet2D>();
        if (bullet != null)
            bullet.SetDamage(currentAttackPower);

        var rb2d = go.GetComponent<Rigidbody2D>();
        if (rb2d != null)
            rb2d.AddForce(direction * currentAttackPower, ForceMode2D.Impulse);
    }
    public void OnShootFrame()
    {
        if (boss == null || bulletPrefab == null) return;

        Vector2 dir = (boss.position - firePoint.position).normalized;
        float offset = 0.3f;

        Vector3 leftFirePos = firePoint.position + firePoint.right * -offset;
        Vector3 rightFirePos = firePoint.position + firePoint.right * offset;

        FireBullet(leftFirePos, dir);
        FireBullet(rightFirePos, dir);
    }

}
