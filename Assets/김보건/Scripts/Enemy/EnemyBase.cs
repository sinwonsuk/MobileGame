using System.Collections;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    //public double maxHp;
    protected double currentHp;

    [Header("HP Bar ")]
    [SerializeField] private GameObject hpBarPrefab;
    private HPBar hpBar;

    [Header("드랍 아이템")]
    [SerializeField] private EnemyDropData dropItem;

    [Header("스폰무적")]
    protected float invincibleTime = 6f;   // 무적 지속시간 (1초)
    private float spawnTime;               // 스폰된 시간


    private Vector3 logicalPosition;  // 이동 좌표 기준
    private Vector3 hitShakeOffset = Vector3.zero;
    private Coroutine shakeCoroutine;
    public Vector3 basePosition;
    private Transform playerTarget;

    protected float moveSpeed = 0.5f;

    protected bool isDead = false;

    // 각 몬스터에서 원하는 값으로 최대체력재정의
    protected virtual float GetMaxHp() => 1.0f;

    //HPBar 계산용
    private float _maxHp;

    protected virtual void Start()
    {
        _maxHp = GetMaxHp();
        currentHp = _maxHp;

        spawnTime = Time.time;

        playerTarget = GameObject.FindGameObjectWithTag("Player")?.transform;

        hpBar = GetComponentInChildren<HPBar>();
        if (hpBarPrefab != null)
        {
            hpBar.SetHP(currentHp, _maxHp);
        }

        logicalPosition = transform.position;
    }

    public virtual void Update()
    {
        if (isDead) return;
        // 내려오는 위치 + 맞을때 흔들림
        //transform.position = basePosition + hitShakeOffset;

        if (playerTarget == null)
        {
            var playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                playerTarget = playerObj.transform;
            }
            else
            {
                return; // 플레이어 없으면 이동 로직 중단
            }
        }

        Vector3 direction = (playerTarget.position - logicalPosition).normalized;
        logicalPosition += direction * moveSpeed * Time.deltaTime;

        // 흔들림이 포함된 실제 위치로 표시
        transform.position = logicalPosition + hitShakeOffset;
    }

    public void SetBasePosition(Vector3 pos)
    {
        basePosition = pos;
        transform.position = basePosition;
    }

    public virtual void TakeDamage(double damage)
    {
        //if (Time.time - spawnTime < invincibleTime) return;

        currentHp -= damage;

        if (hpBar != null)
            hpBar.SetHP(currentHp, _maxHp);


        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);

        shakeCoroutine = StartCoroutine(HitShake());

        Vector3 headPos = transform.position + new Vector3(1.8f, 0, 0);
        DamageTextManager.Instance.ShowDamage(headPos, (float)damage, false);

        if (currentHp <= 0)
        {
            Die();
        }

    }

    protected virtual void Die()
    {
        if (isDead) return; // 중복 방지
        isDead = true;
       // Debug.Log($"{gameObject.name} 죽음");

        //충돌삭제
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;

        FindFirstObjectByType<MonsterSpawner>()?.ResetSpawnFlag();
        DropItem();

        FindAnyObjectByType<MonsterSpawner>()?.MonsterKilled();

        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("DeadTrigger");
        }

        StartCoroutine(FadeOutAndDestroy());

    }

    protected void DropItem()
    {
        if (dropItem == null || dropItem.possibleDrops.Length == 0) return;

        int count = dropItem.dropCount;

        for (int i = 0; i < count; i++)
        {
            int rand = Random.Range(0, dropItem.possibleDrops.Length);
            GameObject dropPrefab = dropItem.possibleDrops[rand];

            GameObject drop = Instantiate(dropPrefab, transform.position, Quaternion.identity);
        }
    }

    protected IEnumerator HitShake()
    {
        float duration = 0.1f;
        float magnitude = 0.1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float offsetX = Random.Range(-1f, 1f) * magnitude;
            float offsetY = Random.Range(-1f, 1f) * magnitude;

            hitShakeOffset = new Vector3(offsetX, offsetY, 0f);
            elapsed += Time.deltaTime;

            yield return null;
        }

        hitShakeOffset = Vector3.zero;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDead) return;

        if (collision.CompareTag("Player"))
        {
            var dungeonManager = FindAnyObjectByType<GameController>().GetManager<DungeonManager>();
            var floorData = dungeonManager.Config.selectedFloorData;

            var spawner = FindFirstObjectByType<MonsterSpawner>();
            if (spawner == null) return;


            spawner.KillAllMonsters(); //남아 있는 몬스터 전부 제거
            spawner.ForceResetWave(); // 전체 상태 초기화
            floorData.ResetStage();   

            ////몬스터 삭제
            //Destroy(gameObject);

            //스폰다시 시작
            spawner.StartMonsterWave();
        }
    }

    private IEnumerator FadeOutAndDestroy()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Destroy(gameObject);
            yield break;
        }

        float duration = 0.5f; // 사라지는 시간
        float elapsed = 0f;
        Color originalColor = sr.color;

        while (elapsed < duration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);

        Destroy(gameObject);
    }

    public virtual void OnDeathAnimationEnd()
    {
       // Destroy(gameObject);
    }

}
