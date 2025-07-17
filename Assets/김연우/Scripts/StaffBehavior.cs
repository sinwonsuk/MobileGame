// == StaffBehavior.cs ==
using UnityEngine;
using System;
using System.Collections;

public class StaffBehavior : MonoBehaviour
{
    StaffStatsSO data;
    [SerializeField] Animator animator;

    // 런타임에 계산된 실제 스탯 (double)
    double currentAttackPower;
    double currentAttackSpeed;

    Transform boss;

    [Header("발사 설정")]
    public GameObject bulletPrefab;
    public Transform firePoint;

    [Header("타겟 필터링")]
    public string bossTag = "a";
    public string bossLayerName = "Boss";

    [Header("감지 범위 (double)")]
    [SerializeField]
    double detectRange = 10.0;

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
        // 레벨에 따라 스탯 재계산 (double)
        currentAttackPower = data.attack_Power
                           + data.attack_PowerPerLevel * (data.level - 1);
        currentAttackSpeed = data.attack_Speed
                           + data.attack_SpeedPerLevel * (data.level - 1);
    }

    private IEnumerator FindAndShoot()
    {
        // 인터벌도 double
        double interval = 1.0 / Math.Max(currentAttackSpeed, 0.01);

        while (true)
        {
            GameObject[] targets = GameObject.FindGameObjectsWithTag(bossTag);
            double minDist = double.PositiveInfinity;
            Transform nearest = null;

            foreach (var t in targets)
            {
                if (t.layer != LayerMask.NameToLayer(bossLayerName))
                    continue;

                // Vector2.Distance 반환이 float라서 double 캐스트
                double dist = Vector2.Distance(firePoint.position, t.transform.position);
                if (dist < minDist && dist <= detectRange)
                {
                    minDist = dist;
                    nearest = t.transform;
                }
            }

            boss = nearest;

            if (boss != null)
                Shoot2D();

            // WaitForSeconds 에는 float 로 캐스팅
            yield return new WaitForSeconds((float)interval);
        }
    }

    private void Shoot2D()
    {
        if (boss == null || bulletPrefab == null)
            return;

        if (animator != null)
            animator.SetTrigger("AttackTrigger");
    }

    private void FireBullet(Vector3 position, Vector2 direction)
    {
        var go = Instantiate(bulletPrefab, position, Quaternion.identity);
        go.transform.right = direction;

        var bullet = go.GetComponent<Bullet2D>();
        if (bullet != null)
            // SetDamage 가 double 인 경우 그 대로 넘겨도 무방
            bullet.SetDamage(currentAttackPower);

        var rb2d = go.GetComponent<Rigidbody2D>();
        if (rb2d != null)
            // AddForce 는 Vector2 * float 를 받으므로 캐스팅
            rb2d.AddForce(direction * (float)currentAttackPower, ForceMode2D.Impulse);
    }

    // 애니메이션 이벤트용 프레임 콜백
    public void OnShootFrame()
    {
        if (boss == null || bulletPrefab == null)
            return;

        Vector2 dir = (boss.position - firePoint.position).normalized;

        // offset 도 double 로 두고, Transform 연산 시 float 로 캐스팅
        double offset = 0.3;
        Vector3 leftFirePos = firePoint.position + firePoint.right * (float)(-offset);
        Vector3 rightFirePos = firePoint.position + firePoint.right * (float)offset;

        FireBullet(leftFirePos, dir);
        FireBullet(rightFirePos, dir);
    }
}
