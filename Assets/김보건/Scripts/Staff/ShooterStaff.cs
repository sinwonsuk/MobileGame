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
    [SerializeField] private double detectRange = 20.0;

    private Transform target;

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

    private bool _inited = false;
    private float nextFireTime = 0f;

    Transform boss;

    public override void Init(StaffStatsSO stats, RuntimeStaffStatsSO Runtimestats)
    {
        base.Init(stats, Runtimestats);
        _inited = true;
        nextFireTime = 0f;
        //StartCoroutine(AutoAttackLoop());
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
        if (skillCooldownBar != null && bigBulletSkill is ICooldownReadable readable && bigBulletSkill is MonoBehaviour mb)
        {
            skillCooldownBar.SetSkill(readable, mb);
        }
    }

    void Update()
    {
        if (!_inited) return;

        if (IsEnemyNearby())
        {
            TryAutoFire();
        }
    }

    private void TryAutoFire()
    {
        if (Time.time < nextFireTime) return;

        target = FindNearestEnemy();
        if (target != null)
        {
            animator?.SetTrigger("AttackTrigger");
            nextFireTime = Time.time + (1f / Mathf.Max((float)currentAttackSpeed, 0.01f));
        }
    }

    private bool IsEnemyNearby()
    {
        Vector3 origin = (firePoint != null) ? firePoint.position : transform.position;
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var e in enemies)
        {
            if (Vector2.Distance(transform.position, e.transform.position) <= detectRange)
                return true;
        }
        return false;
    }

    //private IEnumerator AutoAttackLoop()
    //{
    //    while (true)
    //    {
    //        yield return new WaitForSeconds(1f / Mathf.Max((float)currentAttackSpeed, 0.01f));

    //        target = FindNearestEnemy();
    //        if (target != null)
    //            animator?.SetTrigger("AttackTrigger");
    //    }
    //}

    private Transform FindNearestEnemy()
    {
        Vector3 origin = (firePoint != null) ? firePoint.position : transform.position;
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Transform nearest = null;
        float minDist = Mathf.Infinity;

        foreach (GameObject e in enemies)
        {
            float dist = Vector2.Distance(transform.position, e.transform.position);
            if (dist < minDist && dist <= detectRange)
            {
                minDist = dist;
                nearest = e.transform;
            }
        }

        return nearest;
    }

    // 애니메이션 이벤트에서 호출됨
    public void OnShootFrame()
    {
        if (target == null || bulletPrefab == null || firePoint == null) return;

        Vector2 dir = (target.position - firePoint.position).normalized;
        float offset = 0.3f;
        Vector3 left = firePoint.position + firePoint.right * -offset;
        Vector3 right = firePoint.position + firePoint.right * offset;

        FireBullet(left, dir);
        FireBullet(right, dir);
    }

    private void FireBullet(Vector3 pos, Vector2 dir)
    {
        GameObject go = Instantiate(bulletPrefab, pos, Quaternion.identity);
        go.transform.right = dir;

        if (go.TryGetComponent<Rigidbody2D>(out var rb))
            rb.AddForce(dir * (float)runtimeData.attack_Power, ForceMode2D.Impulse);

        if (go.TryGetComponent<Bullet2D>(out var bullet))
            bullet.SetDamage((float)runtimeData.attack_Power);
    }

    public void ResetCooldown()
    {
        skillCooldown = originalCooldown;
        Debug.Log("쿨타임 복구");
    }
}
