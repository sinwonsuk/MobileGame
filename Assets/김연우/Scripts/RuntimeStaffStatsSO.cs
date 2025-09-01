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

    // RuntimeStaffStatsSO.cs
    public void RecalcWith(StaffStatsSO baseData)
    {
        if (baseData == null) return;

        if (level <= 0)
        {
            switch (baseData.staffType)
            {
                case StaffType.hunter:
                    attack_Power = 0;
                    attack_Speed = 0;
                    break;
                case StaffType.restaurant:
                    timer = 0;
                    cooltime = 0;
                    break;
            }
            isDirty = true;
            return;
        }

        int lv = level;

        switch (baseData.staffType)
        {
            case StaffType.hunter:
                {
                    // 1) 레벨 기본치
                    double atk = baseData.basic_attack_Power + (lv * 1);
                    double aspd = baseData.basic_attack_Speed + (lv * 0.1);

                    // 2) 보유 패시브 합산(전역)
                    var em = EmployeeManager.Instance;
                    if (em != null)
                    {
                        em.GetOwnedPassiveTotals(out _, out var atkSpdMul, out _, out var atkPowMul);
                        atk *= atkPowMul;
                        aspd *= atkSpdMul;
                    }

                    // 3) 최종 저장 (→ 상점 UI가 즉시 버프 포함 수치를 보게 됨)
                    attack_Power = atk;
                    attack_Speed = aspd;
                    break;
                }

            case StaffType.restaurant:
                // 기존 유지
                timer = baseData.basictimer + (lv - 1) * 2f;
                cooltime = baseData.basiccooltime - (lv - 1) * 2f;
                break;
        }
        isDirty = true;
    }


}
