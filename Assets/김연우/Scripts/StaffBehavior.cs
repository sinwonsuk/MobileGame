// == StaffBehavior.cs ==
using UnityEngine;
using System.Collections;

public class StaffBehavior : MonoBehaviour
{
    StaffStatsSO data;

    // 런타임에 계산된 실제 스탯
    int currentAttackPower;
    float currentAttackSpeed;

    Transform boss;

    [Header("발사 설정")]
    public GameObject bulletPrefab;
    public Transform firePoint;

    [Header("타겟 필터링")]
    public string bossTag = "a";
    public string bossLayerName = "Boss";

    public void Init(StaffStatsSO stats)
    {
        data = stats;
        data.level = 1;                       // 최초 레벨 1
        RecalculateStats();
        StartCoroutine(FindAndShoot());
    }

    public void LevelUp()
    {
        data.level++;
        RecalculateStats();
        Debug.Log($"{data.displayName} leveled to {data.level}: " +
                  $"Power={currentAttackPower}, Speed={currentAttackSpeed}");
    }

    void RecalculateStats()
    {
        // 레벨에 따라 스탯 재계산
        currentAttackPower = data.attack_Power
                           + data.attack_PowerPerLevel * (data.level - 1);
        currentAttackSpeed = data.attack_Speed
                           + data.attack_SpeedPerLevel * (data.level - 1);
    }

    private IEnumerator FindAndShoot()
    {
        int bossLayer = LayerMask.NameToLayer(bossLayerName);

        while (boss == null)
        {
            foreach (var c in GameObject.FindGameObjectsWithTag(bossTag))
            {
                if (c.layer == bossLayer)
                {
                    boss = c.transform;
                    break;
                }
            }
            yield return null;
        }

        // 발사 주기 계산
        float interval = 1f / Mathf.Max(currentAttackSpeed, 0.01f);

        while (true)
        {
            Shoot2D();
            yield return new WaitForSeconds(interval);
        }
    }

    private void Shoot2D()
    {
        if (boss == null || bulletPrefab == null) return;

        Vector2 dir = (boss.position - firePoint.position).normalized;
        var go = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        go.transform.right = dir;

        var bullet = go.GetComponent<Bullet2D>();
        if (bullet != null)
            bullet.SetDamage(currentAttackPower);

        var rb2d = go.GetComponent<Rigidbody2D>();
        if (rb2d != null)
            rb2d.AddForce(dir * currentAttackPower, ForceMode2D.Impulse);
    }
}
