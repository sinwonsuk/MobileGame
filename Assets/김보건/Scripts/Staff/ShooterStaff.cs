using System;
using System.Collections;
using UnityEngine;

public class ShooterStaff : StaffBase
{
    [SerializeField] Animator animator;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform firePoint;

    [SerializeField] string bossTag = "a";
    [SerializeField] string bossLayerName = "Boss";
    [SerializeField] private double detectRange = 20.0;

    [Header("스킬")]
    [SerializeField] private GameObject bigBulletSkillPrefab;
    [SerializeField] private float skillCooldown = 5f;
    private float originalCooldown;


    private ISkill bigBulletSkill;
    private SkillCooldownBar skillCooldownBar;

    Transform boss;

    public override void Init(StaffStatsSO stats, RuntimeStaffStatsSO Runtimestats)
    {
        base.Init(stats, Runtimestats);
        StartCoroutine(FindAndShoot());
    }

    public float SkillCooldown
    {
        get => skillCooldown;
        set => skillCooldown = value;
    }


    void Start()
    {
        if (bigBulletSkillPrefab != null)
        {
            var go = Instantiate(bigBulletSkillPrefab, transform);
            bigBulletSkill = go.GetComponent<ISkill>();
        }

        originalCooldown = skillCooldown;

        skillCooldownBar = GetComponentInChildren<SkillCooldownBar>();
        if (skillCooldownBar != null && bigBulletSkill is BigBulletSkill concreteSkill)
            skillCooldownBar.skill = concreteSkill;
    }

    void OnMouseDown()
    {
        Debug.Log("클릭됨");
        if (bigBulletSkill != null && bigBulletSkill.CanCast())
            bigBulletSkill.Cast(firePoint);
    }

    private IEnumerator FindAndShoot()
    {
        while (true)
        {
            // double 타입으로 interval 계산
            double interval = 1.0 / Math.Max(currentAttackSpeed, 0.01);

            // 타겟 검색
            GameObject[] targets = GameObject.FindGameObjectsWithTag(bossTag);
            double minDist = double.PositiveInfinity;
            Transform nearest = null;
            Vector3 origin = firePoint.position;

            foreach (var t in targets)
            {
                if (t.layer != LayerMask.NameToLayer(bossLayerName))
                    continue;

                // double 거리 계산
                Vector3 pos = t.transform.position;
                double dx = pos.x - origin.x;
                double dy = pos.y - origin.y;
                double dist = Math.Sqrt(dx * dx + dy * dy);

                if (dist < minDist && dist <= detectRange)
                {
                    minDist = dist;
                    nearest = t.transform;
                }
            }

            boss = nearest;
            if (boss != null)
                Shoot2D();

            // WaitForSeconds는 float 받으므로 캐스트
            yield return new WaitForSeconds((float)interval);
        }
    }

    private void Shoot2D()
    {
        if (boss == null || bulletPrefab == null) return;
        animator?.SetTrigger("AttackTrigger");
    }

    public void OnShootFrame()  // 애니메이션 이벤트
    {
        if (boss == null || bulletPrefab == null) return;

        Vector2 dir = (boss.position - firePoint.position).normalized;
        double offset = 0.3; // double offset
        Vector3 leftPos = firePoint.position + firePoint.right * (float)-offset;
        Vector3 rightPos = firePoint.position + firePoint.right * (float)offset;

        FireBullet(leftPos, dir);
        FireBullet(rightPos, dir);
    }

    private void FireBullet(Vector3 position, Vector2 direction)
    {
        var go = Instantiate(bulletPrefab, position, Quaternion.identity);
        go.transform.right = direction;

        if (go.TryGetComponent<Bullet2D>(out var bullet))
            bullet.SetDamage(currentAttackPower);

        if (go.TryGetComponent<Rigidbody2D>(out var rb))
            rb.AddForce(direction * (float)currentAttackPower, ForceMode2D.Impulse);
    }

    public void ResetCooldown()
    {
        skillCooldown = originalCooldown;
        Debug.Log("쿨타임 복구");
    }
}
