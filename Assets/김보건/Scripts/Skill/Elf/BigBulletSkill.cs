using System.Collections;
using UnityEngine;

public class BigBulletSkill : MonoBehaviour, ISkill, ICooldownReadable
{
    [SerializeField] private GameObject bigBulletPrefab;
    [SerializeField] private float bulletSpeed = 20f;
    [SerializeField] private int bulletDamage = 100;


    [SerializeField] private float skillCooldown = 3f; 
    [SerializeField] private float detectRange = 25f;

    private float _cooldownProgress = 0f; 
    private float _cooldownSpeed = 1f;      // 로컬 배율
    private int _buffStack = 0;

    private bool _cooldownStartedEarly = false;

    void OnEnable()
    {
        EventBus<CooldownSpeedBuffEvent>.OnEvent += OnBuffEvent;
    }

    void OnDisable()
    {
        EventBus<CooldownSpeedBuffEvent>.OnEvent -= OnBuffEvent;
    }

    void Update()
    {
        // 쿨다운 중이면 진행
        if (_cooldownProgress < skillCooldown)
        {
            _cooldownProgress += Time.deltaTime * _cooldownSpeed;
            if (_cooldownProgress > skillCooldown) _cooldownProgress = skillCooldown;
        }
    }

    public bool CanCast() => _cooldownProgress >= skillCooldown;

    public void Cast(Transform origin)
    {
        if (!CanCast() && !_cooldownStartedEarly) return;

        Transform nearestEnemy = FindNearestEnemy(origin.position);
        Vector3 dir = (nearestEnemy != null)
            ? (nearestEnemy.position - origin.position).normalized
            : origin.up;

        Vector3 pos = origin.position;

        GameObject bullet = Instantiate(bigBulletPrefab, pos, Quaternion.identity);
        bullet.transform.up = dir;
        bullet.transform.localScale *= 2f;

        if (bullet.TryGetComponent<Bullet2D>(out var bulletScript))
            bulletScript.SetDamage(bulletDamage);

        if (bullet.TryGetComponent<Rigidbody2D>(out var rb))
            rb.AddForce(dir * bulletSpeed, ForceMode2D.Impulse);

        // 쿨다운 시작
        _cooldownProgress = 0f;
        _cooldownStartedEarly = false;
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

    // 게이지용 0~1
    public float GetCooldownRatio()
    {
        if (skillCooldown <= 0f) return 1f;
        return Mathf.Clamp01(_cooldownProgress / skillCooldown);
    }

    private void OnBuffEvent(CooldownSpeedBuffEvent e)
    {
        _buffStack++;
        _cooldownSpeed = e.Multiplier;
        StartCoroutine(CoBuffTimer(e.Duration));
    }

    private IEnumerator CoBuffTimer(float duration)
    {
        yield return new WaitForSeconds(duration);
        _buffStack = Mathf.Max(0, _buffStack - 1);
        if (_buffStack == 0) _cooldownSpeed = 1f;
    }

    public void BeginCooldownOnly()
    {
        if (_cooldownProgress >= skillCooldown)
        {
            _cooldownProgress = 0f;
            _cooldownStartedEarly = true; 
        }
    }
}
