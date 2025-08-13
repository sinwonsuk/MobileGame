using UnityEngine;

public class BuffSpeedSkill : MonoBehaviour, ISkill, ICooldownReadable
{
    [Header("버프 설정")]
    [SerializeField] private float baseCooldown = 10f;
    [SerializeField] private float duration = 10f;
    [SerializeField] private float multiplier = 2f;
    [SerializeField] private GameObject buffIconPrefab; // 머리 위 아이콘 프리팹
    [SerializeField] private GameObject buffEffectPrefab;

    private float _progress = 999f; // 시작 시 바로 사용 가능

    public bool CanCast() => _progress >= baseCooldown;

    public void Cast(Transform origin)
    {
        if (!CanCast()) return;

        _progress = 0f;

        // 모든 StaffBase 직원에게 버프 적용
        var staffs = Object.FindObjectsByType<StaffBase>(FindObjectsSortMode.None);
        foreach (var staff in staffs)
        {
            staff.ApplySpeedBuff(multiplier, duration, buffIconPrefab);
            staff.PlayOneShotBuffEffect(buffEffectPrefab, 1f);
        }
    }

    public float GetCooldownRatio()
    {
        if (baseCooldown <= 0f) return 1f;
        return Mathf.Clamp01(_progress / baseCooldown);
    }

    void Update()
    {
        if (_progress < baseCooldown)
        {
            _progress += Time.deltaTime;
            if (_progress > baseCooldown) _progress = baseCooldown;
        }
    }
}