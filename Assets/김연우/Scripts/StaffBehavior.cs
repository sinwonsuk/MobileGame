using UnityEngine;
using UnityEngine.Events;

public class StaffBehavior : MonoBehaviour
{
    // 외부에서 접근 가능한 데이터
    public StaffStatsSO Data { get; private set; }

    [Header("전투용 런타임 스탯")]
    private double currentAttackPower;
    private double currentAttackSpeed;

    [Header("발사 설정 (사냥꾼 타입)")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    [SerializeField] private double detectRange = 10.0;

    [Header("경영용/파견용 이벤트 (Inspector 연결용)")]
    public UnityEvent onRestaurantAction;
    public UnityEvent onDetachmentAction;

    public void Init(StaffStatsSO stats)
    {
        Data = stats;
        Data.level = 1;
        RecalculateStats();
        // Manager에 등록하여 루틴을 제어
        StaffManager.Instance.RegisterStaff(this);
    }

    private void OnDestroy()
    {
        if (StaffManager.Instance != null)
            StaffManager.Instance.UnregisterStaff(this);
    }

    private void RecalculateStats()
    {
        currentAttackPower = Data.attack_Power + Data.attack_PowerPerLevel * (Data.level - 1);
        currentAttackSpeed = Data.attack_Speed + Data.attack_SpeedPerLevel * (Data.level - 1);
    }

    // Manager가 호출하는 메서드
    public void PerformAction()
    {
        switch (Data.staffType)
        {
            case StaffType.hunter:
                FindAndShoot();
                break;
            case StaffType.detachment:
                onDetachmentAction?.Invoke();
                Debug.Log($"{Data.displayName} 파견 액션 수행");
                break;
            case StaffType.restaurant:
                onRestaurantAction?.Invoke();
                Debug.Log($"{Data.displayName} 식당 경영 액션 수행");
                break;
        }
    }

    private void FindAndShoot()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Boss");
        double minDist = double.PositiveInfinity;
        Transform nearest = null;

        foreach (var t in targets)
        {
            if (t.layer != LayerMask.NameToLayer("Boss")) continue;
            double dist = Vector2.Distance(firePoint.position, t.transform.position);
            if (dist < minDist && dist <= detectRange)
            {
                minDist = dist;
                nearest = t.transform;
            }
        }

        if (nearest != null)
            FireBullet(nearest.position);
    }

    private void FireBullet(Vector3 targetPos)
    {
        if (bulletPrefab == null) return;
        var go = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Vector2 dir = (targetPos - firePoint.position).normalized;
        go.transform.right = dir;
        if (go.TryGetComponent<Bullet2D>(out var bullet))
            bullet.SetDamage(currentAttackPower);
    }
    public void LevelUp()
    {
        Data.level++;
        RecalculateStats();
    }
}