using System.Collections;
using UnityEditor.ShaderGraph;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [Header("���� ����")]
    public float maxHp;
    protected float currentHp;

    [Header("HP Bar ")]
    [SerializeField] private GameObject hpBarPrefab;
    private HPBar hpBar;

    [Header("��� ������")]
    [SerializeField] private EnemyDropData dropItem;

    protected virtual void Start()
    {
        currentHp = maxHp;

        hpBar = GetComponentInChildren<HPBar>();
        if (hpBarPrefab != null)
        {
            hpBar.SetHP(currentHp, maxHp);
        }
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
        Debug.Log($"{gameObject.name} ����");
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

        Vector3 originalPos = transform.localPosition;

        while (elapsed < duration)
        {
            float offsetX = Random.Range(-1f, 1f) * magnitude;
            float offsetY = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = originalPos + new Vector3(offsetX, offsetY, 0f);
            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = originalPos;
    }
}
