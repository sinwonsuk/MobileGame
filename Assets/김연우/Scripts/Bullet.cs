// Bullet2D.cs
using UnityEngine;

public class Bullet2D : MonoBehaviour
{
    [Header("����")]
    [Tooltip("�߻� �� �ڵ� ���� �ð�")]
    public float lifeTime = 5f;

    [Tooltip("Ÿ�� �±�")]
    public string targetTag = "a";

    int damage;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void SetDamage(int dmg)
    {
        damage = dmg;
    }

    // Ʈ���� �浹
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(targetTag))
        {
            // ���� ü�� ��ũ��Ʈ ȣ��
            var bossHealth = other.GetComponent<BossHealth>();
            if (bossHealth != null)
                bossHealth.TakeDamage(damage);

            Destroy(gameObject);
            return;
        }

        // �� �� �ٸ� �Ϳ� ��Ƶ� ����
        if (!other.isTrigger)
            Destroy(gameObject);
    }
}
