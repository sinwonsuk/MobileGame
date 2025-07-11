// BossHealth.cs
using UnityEngine;

/// <summary>
/// 간단한 보스 체력 관리 스크립트
/// </summary>
public class BossHealth : MonoBehaviour
{
    [Header("체력 설정")]
    [Tooltip("최대 체력")]
    public int maxHealth = 100;

    private int currentHealth;

    void Awake()
    {
        // 시작 시 현재 체력을 최대치로 설정
        currentHealth = maxHealth;
    }

    /// <summary>
    /// 보스가 데미지를 입습니다.
    /// </summary>
    /// <param name="damage">입을 데미지 값</param>
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage. Current Health: {currentHealth}");

        if (currentHealth <= 0)
            Die();
    }

    /// <summary>
    /// 보스 사망 처리
    /// </summary>
    private void Die()
    {
        // 오브젝트 제거
        Destroy(gameObject);
    }

    /// <summary>
    /// 현재 체력을 반환합니다.
    /// </summary>
    public int GetCurrentHealth()
    {
        return currentHealth;
    }
}