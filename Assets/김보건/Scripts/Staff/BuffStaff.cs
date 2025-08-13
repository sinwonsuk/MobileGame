using System.Collections;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;

public class BuffStaff : StaffBase
{
    [Header("버프 스킬")]
    [SerializeField] private GameObject buffSkillPrefab;
    [SerializeField] private Transform skillOrigin; // 없으면 transform 사용
    [SerializeField] private LayerMask clickMask = ~0;

    [SerializeField] Animator animator;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform firePoint;
    [SerializeField] private double detectRange = 20.0;


    private Transform target;

    private ISkill buffSkill;
    private SkillCooldownBar cooldownBar;


    private InputAction clickAction;
    private bool isShopOpen = false;

    private bool _inited = false;
    private float nextFireTime = 0f;

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
        clickAction.performed += OnPointerPressed;
        clickAction.Enable();
    }

    void OnDisable()
    {
        // EventBus<ShopUIEvent>.OnEvent -= OnShopUIEvent;

        clickAction.performed -= OnPointerPressed;
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

        if (IsEnemyNearby())
        {
            TryAutoFire();
        }
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

        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.VampireBuff, false);

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
        if (target == null || bulletPrefab == null || firePoint == null) return;

        Vector2 dir = (target.position - firePoint.position).normalized;

        FireBullet(firePoint.position, dir);
        //nextFireTime = Time.time + (1f / Mathf.Max((float)currentAttackSpeed, 0.01f));
    }

    private void FireBullet(Vector3 pos, Vector2 dir)
    {
        GameObject go = Instantiate(bulletPrefab, pos, Quaternion.identity);
        //go.transform.right = dir;

        if (go.TryGetComponent<Rigidbody2D>(out var rb))
            rb.AddForce(dir * (float)runtimeData.attack_Power, ForceMode2D.Impulse);

        if (go.TryGetComponent<Bullet2D>(out var bullet))
            bullet.SetDamage((float)runtimeData.attack_Power);
    }

}
