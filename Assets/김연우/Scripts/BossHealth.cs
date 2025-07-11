// BossHealth.cs
using UnityEngine;

/// <summary>
/// ������ ���� ü�� ���� ��ũ��Ʈ
/// </summary>
public class BossHealth : MonoBehaviour
{
    [Header("ü�� ����")]
    [Tooltip("�ִ� ü��")]
    public int maxHealth = 100;

    private int currentHealth;

    void Awake()
    {
        // ���� �� ���� ü���� �ִ�ġ�� ����
        currentHealth = maxHealth;
    }

    /// <summary>
    /// ������ �������� �Խ��ϴ�.
    /// </summary>
    /// <param name="damage">���� ������ ��</param>
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage. Current Health: {currentHealth}");

        if (currentHealth <= 0)
            Die();
    }

    /// <summary>
    /// ���� ��� ó��
    /// </summary>
    private void Die()
    {
        // ������Ʈ ����
        Destroy(gameObject);
    }

    /// <summary>
    /// ���� ü���� ��ȯ�մϴ�.
    /// </summary>
    public int GetCurrentHealth()
    {
        return currentHealth;
    }
}