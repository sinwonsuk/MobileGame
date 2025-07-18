using System.Collections.Generic;
using UnityEngine;

public class StaffManager : MonoBehaviour
{
    public static StaffManager Instance { get; private set; }

    public List<StaffBehavior> staffs = new List<StaffBehavior>();
    public Dictionary<StaffBehavior, float> nextActionTime = new Dictionary<StaffBehavior, float>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void RegisterStaff(StaffBehavior staff)
    {
        if (!staffs.Contains(staff))
        {
            staffs.Add(staff);
            // 첫 실행 시간을 staff.Data.timer 이후로 설정
            nextActionTime[staff] = Time.time + staff.Data.timer;
        }
    }

    public void UnregisterStaff(StaffBehavior staff)
    {
        if (staffs.Remove(staff))
            nextActionTime.Remove(staff);
    }

    private void Update()
    {
        float now = Time.time;
        // Copy 리스트를 사용하여 순회 중 삭제 방지
        foreach (var staff in new List<StaffBehavior>(staffs))
        {
            if (now >= nextActionTime[staff])
            {
                staff.PerformAction();
                // 다음 실행 시간을 timer + cooltime 이후로 설정
                float delay = staff.Data.timer + staff.Data.cooltime;
                nextActionTime[staff] = now + delay;
            }
        }
    }
}