using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaffBase : MonoBehaviour
{
    protected StaffStatsSO data;
    protected RuntimeStaffStatsSO runtimeData;
    protected double currentAttackPower;
    protected double currentAttackSpeed;

    [SerializeField] private GameObject[] buffIconPrefabs; // 다양한 아이콘 (SpeedUp, DefenseUp 등)
    [SerializeField] private Transform iconAnchor;          // 아이콘이 붙을 기준 위치
    private List<GameObject> activeBuffIcons = new();

    private Coroutine _speedBuffCR;

    public virtual void Init(StaffStatsSO stats, RuntimeStaffStatsSO Runtimestats)
    {
        data = stats;
        runtimeData = Runtimestats;

        // staffType 분기 처리
/*        switch (data.staffType)
        {
            case StaffType.hunter:
                if (runtimeData.attack_Power <= 0) runtimeData.attack_Power = data.basic_attack_Power;
                if (runtimeData.attack_Speed <= 0) runtimeData.attack_Speed = data.basic_attack_Speed;
                currentAttackPower = runtimeData.attack_Power;
                currentAttackSpeed = runtimeData.attack_Speed;
                break;

            case StaffType.restaurant:
                if (runtimeData.timer <= 0) runtimeData.timer = data.basictimer;
                if (runtimeData.cooltime <= 0) runtimeData.cooltime = data.basiccooltime;
                break;
        }*/
        SyncFromRuntime();
    }

    public virtual void LevelUp()
    {
        SyncFromRuntime();
    }

    public void SyncFromRuntime()
    {
        switch (data.staffType)
        {
            case StaffType.hunter:
                if (runtimeData.level <= 0)
                {
                    currentAttackPower = 0;
                    currentAttackSpeed = 0;
                    runtimeData.attack_Power = 0;
                    runtimeData.attack_Speed = 0;
                }
                else
                {
                    RecalculateStats();
                    runtimeData.attack_Power = currentAttackPower;
                    runtimeData.attack_Speed = currentAttackSpeed;
                }
                break;

            case StaffType.restaurant:
                if (runtimeData.level <= 0)
                {
                    runtimeData.timer = 0;
                    runtimeData.cooltime = 0;
                }
                else
                {
                    runtimeData.timer = data.basictimer + (runtimeData.level - 1) * 2;
                    runtimeData.cooltime = data.basiccooltime - (runtimeData.level - 1) * 2;
                }
                break;
        }
    }

    protected virtual void RecalculateStats()
    {
        // 기존 Hunter 전용 공식 
        currentAttackPower = data.basic_attack_Power + (runtimeData.level * 1);
        currentAttackSpeed = data.basic_attack_Speed + (runtimeData.level * 0.1);
    }

    public virtual void ApplySpeedBuff(float multiplier, float duration, GameObject iconPrefab)
    {
        // 다른 코루틴(아이콘 제거 포함)까지 끊지 않도록 타겟만 정리
        if (_speedBuffCR != null) StopCoroutine(_speedBuffCR);

        //  아이콘 앵커가 없으면 아이콘은 생략 (버프는 그대로 적용)
        if (iconPrefab != null && iconAnchor != null)
            ShowBuffIcon(iconPrefab, duration);

        // 속도 버프만 처리하는 코루틴
        _speedBuffCR = StartCoroutine(CoSpeedBuff(multiplier, duration));
    }

    private IEnumerator CoSpeedBuff(float multiplier, float duration)
    {
        double originalSpeed = currentAttackSpeed;
        SetAttackSpeed(originalSpeed * multiplier);

        yield return new WaitForSeconds(duration);

        SetAttackSpeed(originalSpeed);
    }

    public void SetAttackSpeed(double newSpeed)
    {
        currentAttackSpeed = newSpeed;
        runtimeData.attack_Speed = newSpeed;
    }

    public void ShowBuffIcon(GameObject iconPrefab, float duration)
    {
        // 안전 가드: 아이콘 프리팹/앵커가 없으면 생성 생략
        if (iconPrefab == null || iconAnchor == null)
            return;

        GameObject icon = Instantiate(iconPrefab, iconAnchor.position, Quaternion.identity, iconAnchor);
        activeBuffIcons.Add(icon);

        // 위치 정렬
        UpdateBuffIconPositions();

        // 자동 제거
        StartCoroutine(CoBuffMark(icon, duration));
    }

    private IEnumerator CoBuffMark(GameObject icon, float duration)
    {
        var disappearAnimTime = 0.5f; // Disappear 애니메이션 길이
        var waitBeforeDisappear = Mathf.Max(0f, duration - disappearAnimTime);

        yield return new WaitForSeconds(waitBeforeDisappear); // 9.5초 기다림

        var anim = icon != null ? icon.GetComponent<Animator>() : null;
        if (anim != null)
            anim.SetTrigger("BuffEnd");

        yield return new WaitForSeconds(disappearAnimTime); // 0.5초 애니메이션 재생
        if (icon != null && activeBuffIcons.Contains(icon))
        {
            activeBuffIcons.Remove(icon);
            Destroy(icon);
            UpdateBuffIconPositions(); // 위치 다시 정렬
        }
    }

    private void UpdateBuffIconPositions()
    {
        //  앵커 없으면 포지션 정렬 시도하지 않음
        if (iconAnchor == null) return;

        float iconSpacing = 0.6f; // 간격
        for (int i = 0; i < activeBuffIcons.Count; i++)
        {
            Vector3 newPos = iconAnchor.position + new Vector3(i * iconSpacing, 0f, 0f);
            if (activeBuffIcons[i] != null)
                activeBuffIcons[i].transform.position = newPos;
        }
    }

    public void PlayOneShotBuffEffect(GameObject effectPrefab, float lifeTime = 1f)
    {
        if (effectPrefab == null) return;

        var fx = Instantiate(effectPrefab, transform.position, Quaternion.identity, transform);

        foreach (var ps in fx.GetComponentsInChildren<ParticleSystem>(true))
            ps.Play(true);

        Destroy(fx, lifeTime);
    }
}
