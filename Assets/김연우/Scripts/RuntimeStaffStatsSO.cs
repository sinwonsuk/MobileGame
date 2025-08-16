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
    [Header("배치 상태")]
    public bool isOwned = false;    // 구매 여부
    public bool isAssigned = false; // 배치 여부
    public int assignedIndex = -1;  // 배치 위치(-1: 미배치, 0/1: 위치)
    [NonSerialized]
    public bool isDirty = false;
    public void RecalcWith(StaffStatsSO baseData)
    {
        if (baseData == null) return;

        int lv = Mathf.Max(1, level);
        switch (baseData.staffType)
        {
            case StaffType.hunter:
                attack_Power = baseData.basic_attack_Power + (lv * 1);
                attack_Speed = baseData.basic_attack_Speed + (lv * 0.1);
                break;

            case StaffType.restaurant:
                timer = baseData.basictimer + (lv - 1) * 2f;
                cooltime = baseData.basiccooltime - (lv - 1) * 2f;
                break;
        }

        isDirty = true;

    }
}
