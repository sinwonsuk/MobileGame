using System.Collections;
using UnityEngine;

public class BigBulletSkill : MonoBehaviour, ISkill
{
    [SerializeField] private GameObject bigBulletPrefab;
    [SerializeField] private float bulletSpeed = 20f;
    [SerializeField] private int bulletDamage = 100;
    [SerializeField] private float skillCooldown = 3f; 
    [SerializeField] private float detectRange = 25f;


    private float lastCastTime = -999f;

    public bool CanCast()
    {
        return Time.time >= lastCastTime + skillCooldown;
    }

    public void Cast(Transform origin)
    {
        if (!CanCast()) return;

        Transform nearestEnemy = FindNearestEnemy(origin.position);


        Vector3 dir;
        if (nearestEnemy != null)
            dir = (nearestEnemy.position - origin.position).normalized;
        else
            dir = origin.up; // 적 없으면 일직선
        Vector3 pos = origin.position;

        GameObject bullet = Instantiate(bigBulletPrefab, pos, Quaternion.identity);
        bullet.transform.right = dir;
        bullet.transform.localScale *= 2f;

        var bulletScript = bullet.GetComponent<Bullet2D>();
        if (bulletScript != null)
            bulletScript.SetDamage(bulletDamage);

        var rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.AddForce(dir * bulletSpeed, ForceMode2D.Impulse);

        lastCastTime = Time.time; // 쿨타임 갱신
    }

    private Transform FindNearestEnemy(Vector3 origin)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float minDist = Mathf.Infinity;
        Transform nearest = null;

        foreach (GameObject e in enemies)
        {
            float dist = Vector2.Distance(origin, e.transform.position);
            if (dist < minDist && dist <= detectRange)
            {
                minDist = dist;
                nearest = e.transform;
            }
        }

        return nearest;
    }

    public float GetCooldownRatio()
    {
        float elapsed = Time.time - lastCastTime;
        return Mathf.Clamp01(elapsed / skillCooldown);
    }

}
