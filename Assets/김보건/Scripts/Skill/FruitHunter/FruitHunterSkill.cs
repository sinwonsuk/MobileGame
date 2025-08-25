using System.Collections;
using UnityEngine;

public class FruitHunterSkill : MonoBehaviour, ISkill, ICooldownReadable
{
    [Header("스킬시간")]
    [SerializeField] private float baseCooldown = 5f;   // 쿨다운
    [SerializeField] private float duration = 5f;        // 강화 유지시간


    [Header("버프 이펙트")]
    [SerializeField] private GameObject buffEffectPrefab;

    [Header("공속")]
    [SerializeField] private float attackSpeedMultiplier = 3f; // 공격 속도 배율

    private float _progress = 0f;
    private FruitHunter _owner;

    private void Awake()
    {
        _owner = GetComponentInParent<FruitHunter>();
    }

    public bool CanCast() => _progress >= baseCooldown;

    public void Cast(Transform origin)
    {
        if (!CanCast()) return;

        _progress = 0f;

        if (_owner == null)
            _owner = origin != null ? origin.GetComponentInParent<FruitHunter>() : GetComponentInParent<FruitHunter>();

        if (_owner != null)
        {
            _owner.EnterSkillMode(duration);

            _owner.SetLocalSpeedMultiplier(attackSpeedMultiplier, duration);

            //_owner.ShowBuffIcon(buffIconPrefab, duration);
            _owner.PlayOneShotBuffEffect(buffEffectPrefab, 1f);

            StartCoroutine(CoDuration());
        }
    }

    private IEnumerator CoDuration()
    {
        yield return new WaitForSeconds(duration);
        _owner?.ExitSkillMode();
    }

    // 쿨다운 진행
    private void Update()
    {
        if (_progress < baseCooldown)
        {
            _progress += Time.deltaTime;
            if (_progress > baseCooldown) _progress = baseCooldown;
        }
    }

    public float GetCooldownRatio()
    {
        if (baseCooldown <= 0f) return 1f;
        return Mathf.Clamp01(_progress / baseCooldown);
    }
}
