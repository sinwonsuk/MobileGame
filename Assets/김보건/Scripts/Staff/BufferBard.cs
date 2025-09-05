using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;

public class BufferBard : StaffBase
{
    [Header("버프 스킬")]
    [SerializeField] private GameObject buffSkillPrefab;
    [SerializeField] private Transform skillOrigin; // 없으면 transform 사용
    [SerializeField] private LayerMask clickMask = ~0;

    [SerializeField] Animator animator;
    [SerializeField] private GameObject[] bulletPrefab;
    [SerializeField] Transform firePoint;
    [SerializeField] private double detectRange = 20.0;

    private SoundManager.sfx[] attackSfx = {
    SoundManager.sfx.BardAttack_one,
    SoundManager.sfx.BardAttack_two,
    SoundManager.sfx.BardAttack_three
    };


    private Transform target;

    private ISkill buffSkill;
    private SkillCooldownBar cooldownBar;


    private InputAction clickAction;
    private bool isShopOpen = false;

    private bool _inited = false;
    private float nextFireTime = 0f;

    private bool _clickQueued;
    private Vector2 _queuedScreenPos;

    private bool _hasLatchedShot;
    private Vector2 _latchedDir;
    private Vector3 _latchedMuzzle;

    public override void Init(StaffStatsSO stats, RuntimeStaffStatsSO Runtimestats)
    {
        base.Init(stats, Runtimestats);
        _inited = true;
        nextFireTime = 0f;
    }

    void OnEnable()
    {
        // EventBus<ShopUIEvent>.OnEvent += OnShopUIEvent;

        clickAction = new InputAction(type: InputActionType.Button, binding: "<Pointer>/press");
        //clickAction.performed += OnPointerPressed;
        clickAction.Enable();
    }

    void OnDisable()
    {
        // EventBus<ShopUIEvent>.OnEvent -= OnShopUIEvent;

        //clickAction.performed -= OnPointerPressed;
        clickAction.Disable();
    }

    void Start()
    {
        if (buffSkillPrefab != null)
        {
            var go = Instantiate(buffSkillPrefab, transform);
            buffSkill = go.GetComponent<ISkill>();
        }

        cooldownBar = GetComponentInChildren<SkillCooldownBar>();
        if (cooldownBar != null && buffSkill is ICooldownReadable readable && buffSkill is MonoBehaviour mb)
            cooldownBar.SetSkill(readable, mb);
    }

    void Update()
    {
        if (!_inited) return;

        if (clickAction != null && clickAction.WasPressedThisFrame() && Pointer.current != null)
        {
            _queuedScreenPos = Pointer.current.position.ReadValue();
            _clickQueued = true;
        }

        // 큐잉된 클릭 처리 (UI/카메라 상태가 안정된 Update 타이밍)
        if (_clickQueued)
        {
            _clickQueued = false;

            // UI 위 클릭이면 무시 (pointerId 전달)
            int pid = Pointer.current != null ? Pointer.current.deviceId : -1;
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(pid))
                return;

            var cam = Camera.main;
            if (cam == null) return;                 // 카메라 가드
            if (!IsFinite(_queuedScreenPos)) return; // 좌표 유효성 검사

            // ScreenPointToRay -> 2D 레이 교차
            var ray = cam.ScreenPointToRay(_queuedScreenPos);
            var hit2D = Physics2D.GetRayIntersection(ray, Mathf.Infinity, clickMask);
            if (hit2D.collider != null)
            {
                var t = hit2D.transform;
                if (t == transform || t.IsChildOf(transform))
                    TryCastBuff();
            }
        }

        if (IsEnemyNearby())
        {
            TryAutoFire();
        }
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

        Vector2 screenPos = Vector2.zero;
        if (Mouse.current != null) screenPos = Mouse.current.position.ReadValue();
        else if (Touchscreen.current != null) screenPos = Touchscreen.current.primaryTouch.position.ReadValue();

        Vector3 world = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0f));
        Vector2 p = new Vector2(world.x, world.y);

        var hits = Physics2D.OverlapPointAll(p, clickMask);
        foreach (var hit in hits)
        {
            if (hit != null && (hit.transform == transform || hit.transform.IsChildOf(transform)))
            {
                TryCastBuff();
                break;
            }
        }
    }

    private void TryCastBuff()
    {
        if (buffSkill == null) return;
        if (!buffSkill.CanCast()) return;

        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.BardBuff, false);

        animator?.SetTrigger("Skill");

        var origin = skillOrigin != null ? skillOrigin : transform;
        buffSkill.Cast(origin);
    }

    private void TryAutoFire()
    {
        if (Time.time < nextFireTime) return;

        target = FindNearestEnemy();
        if (target != null)
        {
            var muzzle = (firePoint != null) ? firePoint.position : transform.position;
            var dir = ((Vector2)(target.position - muzzle)).normalized;

            _latchedMuzzle = muzzle;
            _latchedDir = dir;
            _hasLatchedShot = true;

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
        bool doExtra = ShouldProcExtraNormalShot();

        if (_hasLatchedShot && (target == null || !target.gameObject.activeInHierarchy))
        {
            float off = 0.3f;
            Vector3 rightV = (firePoint != null ? firePoint.right : transform.right);
            Vector3 leftP = _latchedMuzzle - rightV * off;
            Vector3 rightP = _latchedMuzzle + rightV * off;

            if (doExtra) { FireBullet(leftP, _latchedDir); FireBullet(rightP, _latchedDir); }
            else { FireBullet(_latchedMuzzle, _latchedDir); }

            _hasLatchedShot = false;
            return;
        }

        if (target == null || firePoint == null) { _hasLatchedShot = false; return; }

        Vector2 dir = (target.position - firePoint.position).normalized;

        float off2 = 0.3f;
        Vector3 rV = firePoint.right;
        Vector3 lP = firePoint.position - rV * off2;
        Vector3 rP = firePoint.position + rV * off2;

        if (doExtra) { FireBullet(lP, dir); FireBullet(rP, dir); }
        else { FireBullet(firePoint.position, dir); }

        _hasLatchedShot = false;
    }



    private void FireBullet(Vector3 pos, Vector2 dir)
    {
        GameObject prefab = bulletPrefab[Random.Range(0, bulletPrefab.Length)];
        GameObject go = Instantiate(prefab, pos, Quaternion.identity);
        //go.transform.right = dir;

        if (go.TryGetComponent<Rigidbody2D>(out var rb))
            rb.AddForce(dir * (float)runtimeData.attack_Power, ForceMode2D.Impulse);

        if (go.TryGetComponent<Bullet2D>(out var bullet))
            bullet.SetDamage((float)runtimeData.attack_Power);
    }

}
