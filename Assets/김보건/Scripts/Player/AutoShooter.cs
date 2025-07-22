using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.EventSystems;

public class AutoShooter : MonoBehaviour
{
    public Transform firePoint;
    public float fireInterval = 1f;
    public BulletUpgradeManager bulletManager;
    public float enemyDetectRange = 10f; // 감지 범위

    private float timer;

    public Animator animator;
    IShooterState currentState;
    public Player_Battle_IdleState idleState;
    public Player_Battle_AttackState attackState;

    public float manualAttackHoldTime = 0.3f;  // 터치 후 유지 시간

    private bool isShopOpen = false; // UI Shop 열림 여부

    void OnEnable()
    {
        EventBus<ShopUIEvent>.OnEvent += OnShopUIEvent;
    }

    void OnDisable()
    {
        EventBus<ShopUIEvent>.OnEvent -= OnShopUIEvent;
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
    }

    void Update()
    {
        currentState?.Update();

        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (isShopOpen) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (currentState == idleState)
            {
                //SetState(attackState); // 상태 전환
                TouchAttack();         // 발사
            }
            else if (currentState == attackState)
            {
                TouchAttack();         // 연속 발사 가능
                timer = 0f;
            }
        }
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

    void SpawnBullet(Vector2 direction, BulletData data)
    {
        GameObject bulletObj = Instantiate(data.bulletPrefab, firePoint.position, Quaternion.identity);
        BaseBullet bullet = bulletObj.GetComponent<BaseBullet>();
        bullet.Initialize(direction, data.speed, data.damage);
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
        Vector2 shootDirection = Vector2.up;

        SpawnBullet(shootDirection, data);
    }

    //Transform FindEnemy()
    //{
    //    GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

    //    Transform nearest = null;
    //    float minDist = Mathf.Infinity;

    //    foreach (GameObject enemy in enemies)
    //    {
    //        float dist = Vector2.Distance(firePoint.position, enemy.transform.position);
    //        if (dist < minDist && dist <= enemyDetectRange)
    //        {
    //            minDist = dist;
    //            nearest = enemy.transform;
    //        }
    //    }

    //    return nearest;
    //}
}
