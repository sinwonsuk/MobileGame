using UnityEngine;

[CreateAssetMenu(fileName = "StaffStats", menuName = "Clicker/Staff Stats")]
public class StaffStatsSO : ScriptableObject
{
    [Header("직원 기본값")]
    public string indate;     // indate key
    public string displayName;    // 직원 이름
    public StaffType staffType;  //직원 타입 (전투, 경영, 파견)
    [TextArea] public string explain;//설명
    public GameObject itemPrefab;//배치할 오브젝트
    public Sprite portrait; //이미지
    public int baseSalary;  //구매비용
    [Header("전투 직원")]
    public double basic_attack_Power;      // 기본 공격력
    public double basic_attack_Speed;      // 기본 초당 발사 횟수
    [Header("경영직원")]
    public double basictimer;//기본 실행시간
    public double basiccooltime;//기본 쉬는시간
}
