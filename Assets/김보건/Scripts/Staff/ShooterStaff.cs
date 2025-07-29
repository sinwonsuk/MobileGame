using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

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

    // 클릭 판정용
    [SerializeField] private LayerMask clickMask = ~0;

    private ISkill bigBulletSkill;
    private SkillCooldownBar skillCooldownBar;

    private InputAction clickAction;
    private bool isShopOpen = false; //  상점 열리면 입력 무시

    Transform boss;

    public override void Init(StaffStatsSO stats, RuntimeStaffStatsSO Runtimestats)
    {
        base.Init(stats, Runtimestats);
        StartCoroutine(FindAndShoot());
    }

    void OnEnable()
    {
        //EventBus<ShopUIEvent>.OnEvent += OnShopUIEvent;

        clickAction = new InputAction(type: InputActionType.Button, binding: "<Pointer>/press");
        clickAction.performed += OnPointerPressed;
        clickAction.Enable();
    }

    void OnDisable()
    {
        //EventBus<ShopUIEvent>.OnEvent -= OnShopUIEvent;

        clickAction.performed -= OnPointerPressed;
        clickAction.Disable();
    }

    private void OnPointerPressed(InputAction.CallbackContext ctx)
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;
        if (isShopOpen) return;

        // 현재 포인터 스크린 좌표 구하기 (마우스/터치 공통)
        Vector2 screenPos = Vector2.zero;
        if (Mouse.current != null) screenPos = Mouse.current.position.ReadValue();
        else if (Touchscreen.current != null) screenPos = Touchscreen.current.primaryTouch.position.ReadValue();

        // 스크린→월드 변환 후, 포인트 오버랩으로 "내 자신" 클릭인지 확인
        Vector3 world = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0f));
        Vector2 p = new Vector2(world.x, world.y);

        // 한 점에 겹치는 2D 콜라이더들 중에서 내 트랜스폼(또는 자식)을 눌렀는지 확인
        var hits = Physics2D.OverlapPointAll(p, clickMask);
        foreach (var hit in hits)
        {
            if (hit != null && (hit.transform == transform || hit.transform.IsChildOf(transform)))
            {
                TryCastSkill();
                break;
            }
        }
    }

    public void TryCastSkill()
    {
        if (bigBulletSkill == null)
        {
            Debug.LogWarning("[ShooterStaff] bigBulletSkill == null (프리팹/컴포넌트 확인)");
            return;
        }
        if (!bigBulletSkill.CanCast()) return;

        var origin = firePoint != null ? firePoint : transform;
        bigBulletSkill.Cast(origin);
        // Debug.Log("[ShooterStaff] BigBullet Cast!");
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

        skillCooldownBar = GetComponentInChildren<SkillCooldownBar>();
        if (skillCooldownBar != null && bigBulletSkill is BigBulletSkill concrete)
        {
            // 변경된 SkillCooldownBar에 범용 세터 사용
            skillCooldownBar.SetSkill(concrete, concrete);
        }
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
            bullet.SetDamage((float)runtimeData.attack_Power);

        if (go.TryGetComponent<Rigidbody2D>(out var rb))
            rb.AddForce(direction * (float)runtimeData.attack_Power, ForceMode2D.Impulse);
    }

    public void ResetCooldown()
    {
        skillCooldown = originalCooldown;
        Debug.Log("쿨타임 복구");
    }
}
