using UnityEngine;
using System.Collections;

public class BuffSkill : MonoBehaviour, ISkill, ICooldownReadable
{
    [SerializeField] private float baseCooldown = 20f;
    [SerializeField] private float duration = 30f;
    [SerializeField] private float multiplier = 2f;

    private float _progress = 999f; // 시작 즉시 사용 가능
    private float _cooldownSpeed = 1f;
    private int _buffStack = 0;

    void OnEnable()
    {
        // 버프가 자기 자신 쿨다운도 빠르게 만듦
        EventBus<CooldownSpeedBuffEvent>.OnEvent += OnBuffEvent;
    }
    void OnDisable()
    {
        EventBus<CooldownSpeedBuffEvent>.OnEvent -= OnBuffEvent;
    }

    void Update()
    {
        if (_progress < baseCooldown)
        {
            _progress += Time.deltaTime * _cooldownSpeed;
            if (_progress > baseCooldown) _progress = baseCooldown;
        }
    }

    public bool CanCast() => _progress >= baseCooldown;

    public void Cast(Transform origin)
    {
        if (!CanCast()) return;

        // 내 쿨다운 시작
        _progress = 0f;

        // 모든 스킬에 버프 알림
        EventBus<CooldownSpeedBuffEvent>.Raise(new CooldownSpeedBuffEvent
        {
            Multiplier = multiplier,
            Duration = duration
        });
    }

    public float GetCooldownRatio()
    {
        if (baseCooldown <= 0f) return 1f;
        return Mathf.Clamp01(_progress / baseCooldown);
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
}
