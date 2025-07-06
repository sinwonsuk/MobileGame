using System.Collections;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [Header("∞¯≈Î Ω∫≈»")]
    public float maxHp = 10f;
    protected float currentHp;

    [Header("HP Bar ")]
    [SerializeField] private GameObject hpBarPrefab;
    private HPBar hpBarInstance;

    protected virtual void Start()
    {
        currentHp = maxHp;

        if (hpBarPrefab != null)
        {
            GameObject bar = Instantiate(hpBarPrefab, transform.position + new Vector3(0, 1.2f, 0), Quaternion.identity);
            bar.transform.SetParent(transform);
            hpBarInstance = bar.GetComponent<HPBar>();
            hpBarInstance.SetHP(currentHp, maxHp);
        }
    }

    public virtual void TakeDamage(float damage)
    {
        currentHp -= damage;

        if (hpBarInstance != null)
            hpBarInstance.SetHP(currentHp, maxHp);

        StartCoroutine(HitShake());

        if (currentHp <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        Debug.Log($"{gameObject.name} ¡◊¿Ω");
        Destroy(gameObject);
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
