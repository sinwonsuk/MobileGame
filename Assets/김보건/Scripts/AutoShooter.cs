using UnityEngine;

public class AutoShooter : MonoBehaviour
{
    public Transform firePoint;
    public float fireInterval = 1f;
    public BulletUpgradeManager bulletManager;
    public float enemyDetectRange = 10f; // 감지 범위

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (!Input.GetMouseButton(0))
        {
            timer += Time.deltaTime;
            if (timer >= fireInterval)
            {
                AutoFire();
                timer = 0f;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            TouchAttack();
            timer = 0f;
        }
    }

    void TouchAttack()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

        Vector2 shootDirection = worldPos - firePoint.position;

        BulletData data = bulletManager.GetCurrentBullet();
        SpawnBullet(shootDirection, data);
    }

    void AutoFire()
    {
        BulletData data = bulletManager.GetCurrentBullet();

        Vector2 shootDirection = Vector2.up;
        Transform nearestEnemy = FindNearestEnemy();

        if (nearestEnemy != null)
        {
            shootDirection = nearestEnemy.position - firePoint.position;
        }

        SpawnBullet(shootDirection, data);
    }

    void SpawnBullet(Vector2 direction, BulletData data)
    {
        GameObject bulletObj = Instantiate(data.bulletPrefab, firePoint.position, Quaternion.identity);
        BaseBullet bullet = bulletObj.GetComponent<BaseBullet>();
        bullet.Initialize(direction, data.speed);
    }

    Transform FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        Transform nearest = null;
        float minDist = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float dist = Vector2.Distance(firePoint.position, enemy.transform.position);
            if (dist < minDist && dist <= enemyDetectRange)
            {
                minDist = dist;
                nearest = enemy.transform;
            }
        }

        return nearest;
    }
}
