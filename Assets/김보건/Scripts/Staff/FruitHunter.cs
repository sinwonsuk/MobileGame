using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class FruitHunter : StaffBase
{
    [Header("Refs")]
    [SerializeField] private Animator animator;
    [SerializeField] private Transform firePoint;

    [Header("Bullets")]
    [SerializeField] private GameObject normalBulletPrefab;
    [SerializeField] private GameObject skillBulletPrefab;

    [Header("Detect")]
    [SerializeField] private double detectRange = 20.0;

    [Header("Skill")]
    [SerializeField] private GameObject fruitHunterSkillPrefab;   // 프리팹(자기강화)
    [SerializeField] private LayerMask clickMask = ~0;

    private Transform _target;
    private bool _inited;
    private float _nextFireTime;

    // 클릭 큐잉(시뮬레이터/에디터 안전)
    private InputAction _clickAction;
    private bool _clickQueued;
    private Vector2 _queuedScreenPos;

    // 애니메이션/상태
    private bool _skillActive;                 // true면 강화모드
    private static readonly int HashIsSkill = Animator.StringToHash("IsSkill");
    private static readonly int HashAttackTrigger = Animator.StringToHash("AttackTrigger");

    // 쿨다운 UI
    private ISkill _skill;
    private SkillCooldownBar _cooldownBar;

    // 사격 래칭(타겟 상실 시에도 한 발 보장)
    private bool _hasLatchedShot;
    private Vector2 _latchedDir;
    private Vector3 _latchedMuzzle;

    private float _localSpeedMult = 1f;
    private Coroutine _localSpeedCR;

    private double _savedRuntimeSpeed;

    public override void Init(StaffStatsSO stats, RuntimeStaffStatsSO runtimeStats)
    {
        base.Init(stats, runtimeStats);
        _inited = true;
        _nextFireTime = 0f;
    }

    private void Start()
    {
        if (fruitHunterSkillPrefab != null)
        {
            var go = Instantiate(fruitHunterSkillPrefab, transform);
            _skill = go.GetComponent<ISkill>();
        }

        _cooldownBar = GetComponentInChildren<SkillCooldownBar>();
        if (_cooldownBar != null && _skill is ICooldownReadable readable && _skill is MonoBehaviour mb)
            _cooldownBar.SetSkill(readable, mb);
    }

    private void OnEnable()
    {
        _clickAction = new InputAction(type: InputActionType.Button, binding: "<Pointer>/press");
        _clickAction.Enable();
    }

    private void OnDisable()
    {
        if (_clickAction != null)
            _clickAction.Disable();
    }

    private void Update()
    {
        if (!_inited) return;
        if (_clickAction != null && _clickAction.WasPressedThisFrame() && Pointer.current != null)
        {
            _queuedScreenPos = Pointer.current.position.ReadValue();
            _clickQueued = true;
        }
        if (_clickQueued)
        {
            _clickQueued = false;

            // UI 위면 무시
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            var cam = Camera.main;
            if (cam == null) return;

            if (float.IsNaN(_queuedScreenPos.x) || float.IsNaN(_queuedScreenPos.y) ||
                float.IsInfinity(_queuedScreenPos.x) || float.IsInfinity(_queuedScreenPos.y))
                return;

            var ray = cam.ScreenPointToRay(_queuedScreenPos);
            var hit2D = Physics2D.GetRayIntersection(ray, Mathf.Infinity, clickMask);
            if (hit2D.collider != null)
            {
                var t = hit2D.transform;
                if (t == transform || t.IsChildOf(transform))
                    TryCastSkill(); 
            }
        }

        if (IsEnemyNearby())
            TryAutoFire();
    }

    private void TryCastSkill()
    {
        if (_skill == null || !_skill.CanCast()) return;

        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.FruitHunterBuff, false);
        // 스킬은 자기 자신만 강화. 실제 전환은 스킬 프리팹이 담당
        var origin = firePoint != null ? firePoint : transform;
        _skill.Cast(origin);
    }

    private bool IsEnemyNearby()
    {
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
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Transform nearest = null;
        float minDist = Mathf.Infinity;

        foreach (var e in enemies)
        {
            float d = Vector2.Distance(transform.position, e.transform.position);
            if (d < minDist && d <= detectRange)
            {
                minDist = d;
                nearest = e.transform;
            }
        }
        return nearest;
    }

    public void SetLocalSpeedMultiplier(float mult, float duration)
    {
        if (_localSpeedCR != null) StopCoroutine(_localSpeedCR);
        _localSpeedCR = StartCoroutine(CoLocalSpeed(mult, duration));

        float eff = (float)(currentAttackSpeed * mult);
        _nextFireTime = Time.time + (1f / Mathf.Max(eff, 0.01f));

        if (runtimeData != null)
        {
            _savedRuntimeSpeed = runtimeData.attack_Speed;        // 원본 저장
            runtimeData.attack_Speed = currentAttackSpeed * mult; // 유효 공속
            runtimeData.isDirty = true;                            // 갱신 플래그
        }
    }

    private System.Collections.IEnumerator CoLocalSpeed(float mult, float duration)
    {
        _localSpeedMult = mult;                // 스킬 동안 배수 ON
        yield return new WaitForSeconds(duration);
        _localSpeedMult = 1f;                  // 복구
        _localSpeedCR = null;

        if (runtimeData != null)
        {
            runtimeData.attack_Speed = _savedRuntimeSpeed;
            runtimeData.isDirty = true;
        }
    }

    private void TryAutoFire()
    {
        if (Time.time < _nextFireTime) return;

        _target = FindNearestEnemy();
        if (_target != null)
        {
            var muzzle = (firePoint != null) ? firePoint.position : transform.position;
            var dir = ((Vector2)(_target.position - muzzle)).normalized;

            _latchedMuzzle = muzzle;
            _latchedDir = dir;
            _hasLatchedShot = true;

            animator?.SetTrigger(HashAttackTrigger);


            float effectiveSpeed = (float)(currentAttackSpeed * _localSpeedMult);
            _nextFireTime = Time.time + (1f / Mathf.Max(effectiveSpeed, 0.01f));
        }
    }

    public void OnShootFrame()
    {
        if (_hasLatchedShot && (_target == null || !_target.gameObject.activeInHierarchy))
        {
            FireBullet(_latchedMuzzle, _latchedDir);
            _hasLatchedShot = false;
            return;
        }

        if (_target == null || firePoint == null)
        {
            _hasLatchedShot = false;
            return;
        }

        Vector2 dir = (_target.position - firePoint.position).normalized;
        FireBullet(firePoint.position, dir);
        _hasLatchedShot = false;
    }

    private void FireBullet(Vector3 pos, Vector2 dir)
    {
        GameObject prefab = _skillActive ? skillBulletPrefab : normalBulletPrefab;
        if (prefab == null) return;

        GameObject go = Instantiate(prefab, pos, Quaternion.identity);
        // go.transform.right = dir; 
        float damage = (float)runtimeData.attack_Power;
        if (_skillActive)
            damage *= 2f;
        if (go.TryGetComponent<Rigidbody2D>(out var rb))
            rb.AddForce(dir * damage, ForceMode2D.Impulse);

        if (go.TryGetComponent<Bullet2D>(out var bullet))
            bullet.SetDamage(damage);
    }

    // 스킬모드 제어
    public void EnterSkillMode(float duration)
    {
        if (!_skillActive)
        {
            _skillActive = true;
            animator?.SetBool(HashIsSkill, true);   // Idle -> SkillIdle 
        }
    }

    public void ExitSkillMode()
    {
        if (_skillActive)
        {
            _skillActive = false;
            animator?.SetBool(HashIsSkill, false);  // SkillIdle ->Idle
        }
    }
}
