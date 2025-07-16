using UnityEngine;

public class StaffBase : MonoBehaviour
{
    protected StaffStatsSO data;

    protected int currentAttackPower;
    protected float currentAttackSpeed;

    public virtual void Init(StaffStatsSO stats)
    {
        data = stats;
        data.level = 1;
        RecalculateStats();
    }

    public virtual void LevelUp()
    {
        data.level++;
        RecalculateStats();
    }

    protected virtual void RecalculateStats()
    {
        currentAttackPower = data.attack_Power
                           + data.attack_PowerPerLevel * (data.level - 1);
        currentAttackSpeed = data.attack_Speed
                           + data.attack_SpeedPerLevel * (data.level - 1);
    }
}
