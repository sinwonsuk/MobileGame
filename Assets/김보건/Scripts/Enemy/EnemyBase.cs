using System.Collections;
using UnityEditor.ShaderGraph;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [Header("공통 스탯")]
    public float maxHp;
    protected float currentHp;

    [Header("HP Bar ")]
    [SerializeField] private GameObject hpBarPrefab;
    private HPBar hpBar;

    [Header("드랍 아이템")]
    [SerializeField] private EnemyDropData dropItem;

    private Vector3 hitShakeOffset = Vector3.zero;
    public Vector3 basePosition;


    protected virtual void Start()
    {
        currentHp = maxHp;

        hpBar = GetComponentInChildren<HPBar>();
        if (hpBarPrefab != null)
        {
            hpBar.SetHP(currentHp, maxHp);
        }
    }

    public virtual void Update()
    {
        // 내려오는 위치 + 맞을때 흔들림
        transform.position = basePosition + hitShakeOffset;
    }

    public void SetBasePosition(Vector3 pos)
    {
        basePosition = pos;
        transform.position = basePosition;
    }

    public virtual void TakeDamage(float damage)
    {
        currentHp -= damage;

        if (hpBar != null)
            hpBar.SetHP(currentHp, maxHp);

        StartCoroutine(HitShake());

        if (currentHp <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        Debug.Log($"{gameObject.name} 죽음");
        DropItem();
        Destroy(gameObject);
    }

    private void DropItem()
    {
        if (dropItem == null || dropItem.possibleDrops.Length == 0) return;

        int rand = Random.Range(0, dropItem.possibleDrops.Length);

        Instantiate(dropItem.possibleDrops[rand], transform.position, Quaternion.identity);
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
        if (collision.CompareTag("Player"))
        {
            var dungeonManager = FindAnyObjectByType<GameController_bo>().GetManager<DungeonManager>();
            var floorData = dungeonManager.Config.selectedFloorData;

            floorData.ResetStage();               // 스테이지 초기화
            Destroy(gameObject);
            Object.FindFirstObjectByType<MonsterSpawner>().SpawnNextStage();
        }
    }

}
