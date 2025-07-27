using UnityEngine;

public class StaffBase : MonoBehaviour
{
    protected StaffStatsSO data;
    protected RuntimeStaffStatsSO runtimeData;
    protected double currentAttackPower;
    protected double currentAttackSpeed;

    public virtual void Init(StaffStatsSO stats, RuntimeStaffStatsSO Runtimestats)
    {
        data = stats;
        runtimeData = Runtimestats;
        runtimeData.level = 1;

        // staffType 분기 처리
        switch (data.staffType)
        {
            case StaffType.hunter:
                // ▶ 첫 구매: 기본 공격 스탯 할당
                currentAttackPower = data.basic_attack_Power;
                currentAttackSpeed = data.basic_attack_Speed;
                runtimeData.attack_Power = currentAttackPower;
                runtimeData.attack_Speed = currentAttackSpeed;
                break;

            case StaffType.restaurant:
                // ▶ 첫 구매: 기본 타이머/쿨타임 할당
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
                // ▶ 레벨업 시 공격력/속도 재계산
                RecalculateStats();
                runtimeData.attack_Power = currentAttackPower;
                runtimeData.attack_Speed = currentAttackSpeed;
                break;

            case StaffType.restaurant:
                // ▶ 레벨업 시 타이머/쿨타임 +0.1
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
}
