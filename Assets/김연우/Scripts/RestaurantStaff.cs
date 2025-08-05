using UnityEngine;
public class RestaurantStaff : MonoBehaviour
{
    // 직원 데이터 (SO)
    public StaffStatsSO stats;
    public RuntimeStaffStatsSO runtimeStats;

    [Header("요리 시간 감소율 (0.2 = 20%)")]
    public float cookTimeReduction = 0.2f; // Inspector에서 조절 가능

    // 일-휴식 로직
    private bool isWorking = true;
    private double timeCounter;

    private void Start()
    {
        // 일 시작 타이머 초기화
        timeCounter = runtimeStats.timer;
    }

    private void Update()
    {
        timeCounter -= Time.deltaTime;
        if (timeCounter <= 0f)
        {
            isWorking = !isWorking;
            timeCounter = isWorking ? runtimeStats.timer : runtimeStats.cooltime;
        }
    }

    /// <summary>
    /// 현재 근무 중이면, 요리 시간 감소율 반환
    /// </summary>
    public float GetCookTimeReduction()
    {
        return isWorking ? cookTimeReduction : 0f;
    }

    /// <summary>
    /// 현재 근무 중인지 여부
    /// </summary>
    public bool IsWorking()
    {
        return isWorking;
    }
}
