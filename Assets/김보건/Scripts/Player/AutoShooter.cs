using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class AutoShooter : MonoBehaviour
{
    public Transform firePoint;
    public float fireInterval = 1f;
    public BulletUpgradeManager bulletManager;
    public float enemyDetectRange = 10f; // 감지 범위

    private float timer;

    public PlayerStatData playerStats;

    public Animator animator;
    IShooterState currentState;
    public Player_Battle_IdleState idleState;
    public Player_Battle_AttackState attackState;

    public float manualAttackHoldTime = 0.3f;  // 터치 후 유지 시간

    private bool isShopOpen = false; // UI Shop 열림 여부

    private InputAction attackAction;

    private bool _pendingAttack;

    private bool _isInDungeon = false;

    public bool IsAttackPressed => attackAction != null && attackAction.ReadValue<float>() > 0;

    private float _lastManualAttackTime = -999f; // 마지막 수동 발사 시각

    void OnEnable()
    {
        EventBus<ShopUIEvent>.OnEvent += OnShopUIEvent;
        EventBus<StatChangedEvent>.OnEvent += OnStatChanged;


        EventBus<LocationChangedEvent>.OnEvent += OnLocationChanged;
        _isInDungeon = (LocationState.Current == location.Dungeon);

        attackAction = new InputAction(type: InputActionType.Button, binding: "<Pointer>/press"); // 마우스와 터치 대응
        attackAction.performed += ctx => { _pendingAttack = true; };
        attackAction.Enable();


    }

    void OnDisable()
    {
        EventBus<ShopUIEvent>.OnEvent -= OnShopUIEvent;
        EventBus<StatChangedEvent>.OnEvent -= OnStatChanged;

        EventBus<LocationChangedEvent>.OnEvent -= OnLocationChanged;

        attackAction.performed -= ctx => { _pendingAttack = true; };
        attackAction.Disable();
    }

    private void OnLocationChanged(LocationChangedEvent e)
    {
        _isInDungeon = (e.value == location.Dungeon);
    }


    private void OnAttackInput(InputAction.CallbackContext context)
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (isShopOpen) return;

        if (currentState == idleState)
        {
            TouchAttack();
        }
        else if (currentState == attackState)
        {
            TouchAttack();
            timer = 0f;
        }
    }

    private void OnShopUIEvent(ShopUIEvent evt)
    {
        isShopOpen = evt.isShopOpen;
    }

    void Start()
    {

        idleState = new Player_Battle_IdleState(this);
        attackState = new Player_Battle_AttackState(this);
        SetState(idleState); // 처음엔 idle 상태

        fireInterval = playerStats.autoAttackInterval;

        if (bulletManager != null && bulletManager.GetCurrentBullet() != null)
        {
           // bulletManager.GetCurrentBullet().damage = playerStats.attackPower;
            bulletManager.UpdateBulletByLevel(playerStats.level, playerStats.attackPower);
        }
    }

    void Update()
    {
        currentState?.Update();

        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (!_isInDungeon) return;
        if (isShopOpen) return;

        if (_pendingAttack)
        {
            _pendingAttack = false;

            if (isShopOpen) return;
            if (IsPointerOverUI()) return; // 최신 포인터 위치로 UI 히트 체크


            float manualCooldown = playerStats.autoAttackInterval / 3f;
            if (Time.time - _lastManualAttackTime < manualCooldown)
                return;


            if (currentState == idleState)
            {
                TouchAttack();
            }
            else if (currentState == attackState)
            {
                TouchAttack();
                timer = 0f;
            }

            _lastManualAttackTime = Time.time;
        }
    }

    private bool IsPointerOverUI()
    {
        if (EventSystem.current == null) return false;

        Vector2 pos = Pointer.current != null
            ? Pointer.current.position.ReadValue()
            : (Vector2)Input.mousePosition; // 폴백

        var data = new PointerEventData(EventSystem.current) { position = pos };
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(data, results);
        return results.Count > 0;
    }

    public void SetState(IShooterState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }


    void TouchAttack()
    {
        //Vector3 mousePos = Input.mousePosition;
        //Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

        // 애니메이션 즉시 첫 프레임부터
        animator.Play("Player_Battle_Attack", 0, 0f);

        //animator.ResetTrigger("AttackTrigger");  // 중복 입력 시 애니메이션 끊기 방지
        //animator.SetTrigger("AttackTrigger");

        //Vector2 shootDirection = Vector2.up;

        //BulletData data = bulletManager.GetCurrentBullet();
        //SpawnBullet(shootDirection, data);

        // FSM 공격 상태가 아니면 한 번만 진입
        if (currentState == idleState)
            SetState(attackState);
    }

    public void PlayAttackAnimationImmediately()
    {
        animator.Play("Player_Battle_Attack", 0, 0f); // 첫 프레임부터 즉시 재생
    }

    void AutoFire()
    {
        animator.Play("Player_Battle_Attack", 0, 0f);

        //BulletData data = bulletManager.GetCurrentBullet();

        //Vector2 shootDirection = Vector2.up;
        //SpawnBullet(shootDirection, data);

        if (currentState == idleState)
            SetState(attackState);
    }

    public void TryAutoFire()
    {
        timer += Time.deltaTime;
        if (timer >= fireInterval)
        {
            AutoFire();
            timer = 0f;
        }
    }

    public void SetAttackAnimation(bool isAttacking)
    {
        animator.SetBool("Attack", isAttacking);
    }

    // ======== 크리티컬 판정 + 최종 데미지 계산 ========
    private (float damage, bool isCrit) ComputeShotDamage()
    {
        float baseDamage = (playerStats != null) ? playerStats.attackPower : 1f;
        float chancePct = (playerStats != null) ? playerStats.critChance : 0f;             // %
        float critMult = (playerStats != null) ? playerStats.critDamageMultiplier : 1f;   // 배수

        bool isCrit = UnityEngine.Random.value < (chancePct * 0.01f);
        float finalDamage = isCrit ? baseDamage * critMult : baseDamage;
        return (finalDamage, isCrit);
    }

    void SpawnBullet(Vector2 direction, BulletData data)
    {
        GameObject bulletObj = Instantiate(data.bulletPrefab, firePoint.position, Quaternion.identity);
        BaseBullet bullet = bulletObj.GetComponent<BaseBullet>();
        bullet.Initialize(direction, data.speed, data.damage);

    }

    // ======== 데미지 오버라이드 가능한 스폰 오버로드(크리 반영용) ========
    void SpawnBullet(Vector2 direction, BulletData data, float damageOverride)
    {
        GameObject bulletObj = Instantiate(data.bulletPrefab, firePoint.position, Quaternion.identity);
        BaseBullet bullet = bulletObj.GetComponent<BaseBullet>();
        bullet.Initialize(direction, data.speed, damageOverride);
        // 크리 연출 원하면 bullet.SetCrit(true/false) 같은 훅 추가해서 여기서 호출
    }

    public bool IsEnemyNearby()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var enemy in enemies)
        {
            if (Vector2.Distance(firePoint.position, enemy.transform.position) <= enemyDetectRange)
                return true;
        }
        return false;
    }

    public void SetAttackTrigger()
    {
        animator.SetTrigger("AttackTrigger");
    }

    public void OnAttackAnimationEnd()
    {
        if (!IsEnemyNearby()) // 자동공격 아닐 때만 FSM 전환
        {
            SetState(idleState);
        }
    }

    public void OnShootFrame() // 애니메이션 호출 함수
    {
        BulletData data = bulletManager.GetCurrentBullet();

        Vector2 shootDirection;

        Transform target = FindEnemy();
        if (target != null)
            shootDirection = (target.position - firePoint.position).normalized;
        else
            shootDirection = Vector2.up; // 적 없으면 위로

        // 크리티컬 판정 및 최종 데미지 계산
        var (finalDamage, isCrit) = ComputeShotDamage();

        // 크리 반영된 데미지로 발사
        SpawnBullet(shootDirection, data, finalDamage);
        // 원하면 크리 트리거/이펙트: if (isCrit) animator.SetTrigger("Crit");
    }

    void OnStatChanged(StatChangedEvent evt)
    {
        // 전체 갱신
        fireInterval = playerStats.autoAttackInterval;

        if (bulletManager != null && bulletManager.GetCurrentBullet() != null)
        {
            bulletManager.UpdateBulletByLevel(playerStats.level, playerStats.attackPower);
        }
    }

    Transform FindEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        Transform nearest = null;
        float minDist = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float dist = Vector2.Distance(firePoint.position, enemy.transform.position);
            if (dist < minDist && dist <= enemyDetectRange)
            {
                minDist = dist;
                nearest = enemy.transform;
            }
        }

        return nearest;
    }
}