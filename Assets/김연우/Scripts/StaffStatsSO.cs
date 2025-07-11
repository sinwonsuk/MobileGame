// == StaffStatsSO.cs ==
using UnityEngine;

[CreateAssetMenu(fileName = "StaffStats", menuName = "Clicker/Staff Stats")]
public class StaffStatsSO : ScriptableObject
{
    [Header("Identity")]
    public string employeeId;     // indate key
    public string displayName;    // 직원 이름

    [Header("Runtime Level")]
    [Tooltip("현재 레벨 (런타임에 Init 시 1로 설정됩니다)")]
    public int level;

    [Header("Base Stats")]
    public Sprite portrait;
    public int baseSalary;
    public int attack_Power;      // 레벨 1 기준 공격력
    public int attack_Speed;      // 레벨 1 기준 초당 발사 횟수

    [Header("Per-Level Growth")]
    [Tooltip("레벨업 시 추가되는 공격력")]
    public int attack_PowerPerLevel = 1;
    [Tooltip("레벨업 시 추가되는 공격속도")]
    public int attack_SpeedPerLevel = 1;

    [Header("Other")]
    [TextArea] public string explain;
    public GameObject itemPrefab;
}
