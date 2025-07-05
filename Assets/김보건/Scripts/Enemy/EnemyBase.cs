using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [Header("���� ����")]
    public float maxHp = 10f;
    protected float currentHp;

    protected virtual void Start()
    {
        currentHp = maxHp;
    }

    public virtual void TakeDamage(float damage)
    {
        currentHp -= damage;

        if (currentHp <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        Debug.Log($"{gameObject.name} ����");
        Destroy(gameObject);
    }
}
