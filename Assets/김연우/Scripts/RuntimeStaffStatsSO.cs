using System;
using UnityEngine;

[CreateAssetMenu(fileName = "StaffStats", menuName = "Clicker/RuntimeStaff Stats")]
public class RuntimeStaffStatsSO : ScriptableObject
{
    [Header("직원 기본값")]
    public string indate;     // indate key
    public string displayName;    // 직원 이름
    public StaffType staffType;  //직원 타입 (전투, 경영, 파견)
    public int level;// (레벨)
    [Header("전투 직원")]
    public double attack_Power;      // 현재 공격력
    public double attack_Speed;      // 현재 초당 발사 횟수
    [Header("경영직원")]
    public double timer;//현재 실행시간
    public double cooltime;// 현재 쉬는시간

    [NonSerialized]
    public bool isDirty = false;
}
