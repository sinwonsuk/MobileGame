using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStatData", menuName = "Stat/PlayerStatData")]
public class PlayerStatData : ScriptableObject
{

    public float basicAtk = 1f;

    // 파생(런타임) 값들
    public int level;                   // 레벨
    public float attackPower;             // 기본공격레벨
    public float critChance;              // 크리티컬
    public float autoAttackInterval;      // 자동공격속도 (낮을수록 빠름)
    public float critDamageMultiplier;    // 크리티컬데미지

    // ---- 튜닝 파라미터(인스펙터에서 조절 가능) ----
    [Header("기둥 공식: basicAtk ↔ level")]
    public float baseAttack = 1f;   // L=1일 때 공격력 기준
    public float attackPerLevel = 1f;    // 레벨당 공격력 증가량
    public int minLevel = 1;

    [Header("파생 공식(레벨 기반)")]
    public float baseCritChance = 5f;    // 기본크리
    public float critChancePerLevel = 0.5f;  // 레벨업당 크리증가
    public float baseAutoInterval = 1.5f;  // 기본 자동공격속도
    public float intervalReducePerLv = 0.03f; //레벨업당 자동공격속도 증가
    public float minAutoInterval = 0.2f;  // 자동공격속도 제한
    public float baseCritMult = 1.5f;  // 크리티컬 데미지
    public float critMultPerLevel = 0.05f; // 레벨업당 크리티컬 데미지 증가량


    public void RecalculateFromBasicAtk()
    {
        int L = Mathf.FloorToInt((basicAtk - baseAttack) / attackPerLevel) + 1;
        level = Mathf.Max(minLevel, L);

        attackPower = basicAtk;

        critChance = Mathf.Clamp(baseCritChance + critChancePerLevel * (level-1), 0f, 100f);

        autoAttackInterval = Mathf.Max(minAutoInterval,
            baseAutoInterval - intervalReducePerLv * (level - 1));

        critDamageMultiplier = baseCritMult + critMultPerLevel * (level - 1);
    }

    public void ApplyLevelUp(int addLevels = 1)
    {
        int newLevel = Mathf.Max(minLevel, level + addLevels);

        basicAtk = baseAttack + attackPerLevel * (newLevel - 1);

        RecalculateFromBasicAtk();
    }
}