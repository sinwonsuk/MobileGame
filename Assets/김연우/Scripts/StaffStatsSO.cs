
using UnityEngine;

[CreateAssetMenu(fileName = "StaffStats", menuName = "Clicker/Staff Stats")]
public class StaffStatsSO : ScriptableObject
{
    [Header("Identity")]
    public string indate;     // indate key
    public string displayName;    // 직원 이름
    public StaffType staffType;  //직원 타입 (전투, 경영, 파견)
    [Header("Runtime Level")]
    public int level;// (레벨)

    [Header("Base Stats")]
    public Sprite portrait; //이미지
    public int baseSalary;  //
    public double attack_Power;      // 레벨 1 기준 공격력
    public double attack_Speed;      // 레벨 1 기준 초당 발사 횟수

    [Header("Per-Level Growth")]
    [Tooltip("레벨업 시 추가되는 공격력")]
    public double attack_PowerPerLevel = 1;
    [Tooltip("레벨업 시 추가되는 공격속도")]
    public double attack_SpeedPerLevel = 1;
    [Tooltip("레벨업 시 시간 단축")]
    public float timer;//실행시간
    public float cooltime;//쉬는시간
    [Header("Other")]
    [TextArea] public string explain;//설명
    public GameObject itemPrefab;//배치할 오브젝트
}
