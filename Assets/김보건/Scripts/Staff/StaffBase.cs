using UnityEngine;

public class StaffBase : MonoBehaviour
{
    StaffStatsSO data;
    RuntimeStaffStatsSO Runtimedata;
    protected double currentAttackPower;
    protected double currentAttackSpeed;

    public virtual void Init(StaffStatsSO stats, RuntimeStaffStatsSO Runtimestats) 
    {
        data = stats;
        Runtimedata = Runtimestats;
        Runtimedata.level = 1;
        RecalculateStats();
    }

    public virtual void LevelUp()
    {
        Runtimedata.level++;
        RecalculateStats();
    }

    protected virtual void RecalculateStats()
    {
        currentAttackPower = data.basic_attack_Power +  (Runtimedata.level * 0.1);
        currentAttackSpeed = data.basic_attack_Speed + (Runtimedata.level - 1);
    }
}
