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

    private bool _pendingSkillCast = false;

    Transform boss;

    private bool _clickQueued;
    private Vector2 _queuedScreenPos;

    private bool _hasLatchedShot;
    private Vector2 _latchedDir;
    private Vector3 _latchedLeft, _latchedRight;

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
        clickAction.performed += ctx =>
        {
            // 콜백에서는 좌표만 큐잉
            if (Pointer.current != null)
            {
                _queuedScreenPos = Pointer.current.position.ReadValue();
                _clickQueued = true;
            }
        };
        clickAction.Enable();
    }

    void OnDisable()
    {
        //EventBus<ShopUIEvent>.OnEvent -= OnShopUIEvent;

        clickAction.performed -= OnPointerPressed;
        clickAction.Disable();
    }

    private static bool IsFinite(Vector2 v)
    {
        return !(float.IsNaN(v.x) || float.IsNaN(v.y) ||
                 float.IsInfinity(v.x) || float.IsInfinity(v.y));
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

        TryCastSkillByClick();
    }

    private void TryCastSkillByClick()
    {
        if (bigBulletSkill == null) return;
        if (!bigBulletSkill.CanCast()) return;

        _pendingSkillCast = true;          // 스킬 발사 예약
        (bigBulletSkill as BigBulletSkill)?.BeginCooldownOnly();

        // 평타 애니메이션을 첫 프레임부터 즉시 재생 (평타 중이어도 끊고 시작)
        animator.Play("Attack", 0, 0f);

        // 평타 자동 사격 템포 잠깐 멈춰 중복 방지
        nextFireTime = Time.time + 0.15f;
    }

    public void TryCastSkill()
    {
        if (bigBulletSkill == null)
        {
            Debug.LogWarning("[ShooterStaff] bigBulletSkill == null (프리팹/컴포넌트 확인)");
            return;
        }
        if (!bigBulletSkill.CanCast()) return;

        //var origin = firePoint != null ? firePoint : transform;
        //bigBulletSkill.Cast(origin);
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

        if (_clickQueued)
        {
            _clickQueued = false;

            int pid = Pointer.current != null ? Pointer.current.deviceId : -1;
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(pid))
                return;

            var cam = Camera.main;
            if (cam == null) return;
            if (!IsFinite(_queuedScreenPos)) return;

            var ray = cam.ScreenPointToRay(_queuedScreenPos);
            var hit2D = Physics2D.GetRayIntersection(ray, Mathf.Infinity, clickMask);
            if (hit2D.collider != null)
            {
                var t = hit2D.transform;
                if (t == transform || t.IsChildOf(transform))
                    TryCastSkillByClick();
            }
        }

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
            var origin = (firePoint != null) ? firePoint.position : transform.position;
            var dir = ((Vector2)(target.position - origin)).normalized;
            float offset = 0.3f;
            _latchedLeft = (firePoint != null ? firePoint.position : transform.position) + firePoint.right * -offset;
            _latchedRight = (firePoint != null ? firePoint.position : transform.position) + firePoint.right * offset;
            _latchedDir = dir;
            _hasLatchedShot = true;

            animator?.SetTrigger("AttackTrigger");
            //nextFireTime = Time.time + (1f / Mathf.Max((float)currentAttackSpeed, 0.01f));
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
        if (_pendingSkillCast)
        {
            var origin = firePoint != null ? firePoint : transform;
            bigBulletSkill.Cast(origin);       // 쿨타임도 여기서 시작
            _pendingSkillCast = false;
            return;
        }

        if ((target == null || !target.gameObject.activeInHierarchy) && _hasLatchedShot)
        {
            FireBullet(_latchedLeft, _latchedDir);
            FireBullet(_latchedRight, _latchedDir);
            _hasLatchedShot = false;
            return;
        }

        if (target == null || bulletPrefab == null || firePoint == null)
        {
            _hasLatchedShot = false;
            return;
        }

        Vector2 dir = (target.position - firePoint.position).normalized;
        float offset = 0.3f;
        Vector3 left = firePoint.position + firePoint.right * -offset;
        Vector3 right = firePoint.position + firePoint.right * offset;

        FireBullet(left, dir);
        FireBullet(right, dir);

        //nextFireTime = Time.time + (1f / Mathf.Max((float)currentAttackSpeed, 0.01f));
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
