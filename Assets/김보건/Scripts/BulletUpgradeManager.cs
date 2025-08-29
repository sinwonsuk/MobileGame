using UnityEngine;

public class BulletUpgradeManager : MonoBehaviour
{
    [Header("레벨별 총알")]
    public BulletData[] tiers;              // [0]=Lv1~, [1]=Lv10~, [2]=Lv20~, [3]=Lv30~, [4]=Lv40~
    public int[] levelThresholds = { 1, 10, 20, 30, 40 };

    [Tooltip("현재 사용중인 총알")]
    public BulletData currentBullet;

    public void SetBullet(BulletData newBullet) => currentBullet = newBullet;

    public BulletData GetCurrentBullet() => currentBullet;

    public BulletData GetBulletForLevel(int level)
    {
        for (int i = levelThresholds.Length - 1; i >= 0; --i)
        {
            if (level >= levelThresholds[i] && i < tiers.Length && tiers[i] != null)
                return tiers[i];
        }
        return (tiers != null && tiers.Length > 0 && tiers[0] != null) ? tiers[0] : currentBullet;
    }

    // 레벨 입력 받아 현재 총알 갱신
    public void UpdateBulletByLevel(int level, float newDamage)
    {
        var next = GetBulletForLevel(level);
        if (next != null)
        {
            //next.damage = newDamage;
            SetBullet(next);
        }
    }
}
