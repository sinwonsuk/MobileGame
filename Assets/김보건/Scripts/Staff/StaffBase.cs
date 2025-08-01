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
    [SerializeField] private Transform iconAnchor; // 아이콘이 붙을 기준 위치
    private List<GameObject> activeBuffIcons = new();

    private Coroutine _speedBuffCR;

    public virtual void Init(StaffStatsSO stats, RuntimeStaffStatsSO Runtimestats)
    {
        data = stats;
        runtimeData = Runtimestats;

        // staffType 분기 처리
        switch (data.staffType)
        {
            case StaffType.hunter:
                // 첫 구매: 기본 공격 스탯 할당
                currentAttackPower = data.basic_attack_Power;
                currentAttackSpeed = data.basic_attack_Speed;
                runtimeData.attack_Power = currentAttackPower;
                runtimeData.attack_Speed = currentAttackSpeed;
                break;

            case StaffType.restaurant:
                // 첫 구매: 기본 타이머/쿨타임 할당
                runtimeData.timer = data.basictimer;     
                runtimeData.cooltime = data.basiccooltime;  
                break;
        }
    }

    public virtual void LevelUp()
    {
        runtimeData.level++;

        switch (data.staffType)
        {
            case StaffType.hunter:
                // 레벨업 시 공격력/속도 재계산
                RecalculateStats();
                runtimeData.attack_Power = currentAttackPower;
                runtimeData.attack_Speed = currentAttackSpeed;
                break;

            case StaffType.restaurant:
                // 레벨업 시 타이머/쿨타임 +0.1
                runtimeData.timer += 0.1;
                runtimeData.cooltime -= 0.1;
                break;
        }
    }

    protected virtual void RecalculateStats()
    {
        // 기존 Hunter 전용 공식 
        currentAttackPower = data.basic_attack_Power + (runtimeData.level * 0.1);
        currentAttackSpeed = data.basic_attack_Speed + (runtimeData.level * 0.1);
    }

    public virtual void ApplySpeedBuff(float multiplier, float duration, GameObject iconPrefab)
    {
        // 다른 코루틴(아이콘 제거 포함)까지 끊지 않도록 타겟만 정리
        if (_speedBuffCR != null) StopCoroutine(_speedBuffCR);

        // 아이콘 앵커에 생성
        if (iconPrefab != null) ShowBuffIcon(iconPrefab, duration);

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

        var anim = icon.GetComponent<Animator>();
        if (anim != null)
            anim.SetTrigger("BuffEnd");

        yield return new WaitForSeconds(disappearAnimTime); // 0.5초 애니메이션 재생
        if (activeBuffIcons.Contains(icon))
        {
            activeBuffIcons.Remove(icon);
            Destroy(icon);
            UpdateBuffIconPositions(); // 위치 다시 정렬
        }
    }

    private void UpdateBuffIconPositions()
    {
        float iconSpacing = 0.6f; // 간격
        for (int i = 0; i < activeBuffIcons.Count; i++)
        {
            Vector3 newPos = iconAnchor.position + new Vector3(i * iconSpacing, 0f, 0f);
            activeBuffIcons[i].transform.position = newPos;
        }
    }
}
