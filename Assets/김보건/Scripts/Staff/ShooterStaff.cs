using System.Collections;
using UnityEngine;

public class ShooterStaff : StaffBase
{
    [SerializeField] Animator animator;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform firePoint;

    [SerializeField] string bossTag = "a";
    [SerializeField] string bossLayerName = "Boss";
    [SerializeField] float detectRange = 20f;

    [Header("스킬")]
    [SerializeField] private GameObject bigBulletSkillPrefab;
    [SerializeField] private float skillCooldown = 5f;

    private ISkill bigBulletSkill;
    private SkillCooldownBar skillCooldownBar;
    private bool isSkillReady = true;

    Transform boss;

    public override void Init(StaffStatsSO stats)
    {
        base.Init(stats);
        StartCoroutine(FindAndShoot());
    }
    void Start()
    {
        if (bigBulletSkillPrefab != null)
        {
            var go = Instantiate(bigBulletSkillPrefab, transform);
            bigBulletSkill = go.GetComponent<ISkill>();
        }

        skillCooldownBar = GetComponentInChildren<SkillCooldownBar>();

        if (skillCooldownBar != null && bigBulletSkill is BigBulletSkill concreteSkill)
        {
            skillCooldownBar.skill = concreteSkill;
        }
    }

    void OnMouseDown()
    {
        Debug.Log("클릭됨");
        if (bigBulletSkill != null && bigBulletSkill.CanCast())
            bigBulletSkill.Cast(firePoint);
    }

    private IEnumerator FindAndShoot()
    {
        float interval = 1f / Mathf.Max((float)currentAttackSpeed, 0.01f);

        while (true)
        {
            GameObject[] targets = GameObject.FindGameObjectsWithTag(bossTag);
            float minDist = Mathf.Infinity;
            Transform nearest = null;

            foreach (var t in targets)
            {
                Debug.Log($"[타겟 후보] 이름: {t.name}, 태그: {t.tag}, 레이어: {LayerMask.LayerToName(t.layer)}");
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
            animator.SetTrigger("AttackTrigger"); // 애니메이션 트리거
    }

    public void OnShootFrame()  // 애니메이션 이벤트로 호출
    {
        if (boss == null || bulletPrefab == null) return;

        Vector2 dir = (boss.position - firePoint.position).normalized;
        float offset = 0.3f;

        Vector3 leftFirePos = firePoint.position + firePoint.right * -offset;
        Vector3 rightFirePos = firePoint.position + firePoint.right * offset;

        FireBullet(leftFirePos, dir);
        FireBullet(rightFirePos, dir);
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
            rb2d.AddForce(direction * (float)currentAttackPower, ForceMode2D.Impulse);
    }

    //private void OnMouseDown()
    //{
    //    if (!isSkillReady) return;
    //    StartCoroutine(SkillRoutine());
    //}

}
