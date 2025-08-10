using UnityEngine;

/// <summary>
/// Restaurant 직원. 근무/휴식 주기 관리 & 요리 시간 감소 효과 이벤트 발송.
/// </summary>
public class RestaurantStaff_Two : StaffBase
{
    [Header("직원 데이터 (SO)")]
    public StaffStatsSO stats;
    public RuntimeStaffStatsSO runtimeStats;

    [Header("요리 시간 감소율 (0.2 = 20%)")]
    public float cookTimeReduction = 0.9f; // Inspector에서 조절

    private bool isWorking = true;  // 근무 중 여부
    private double timeCounter;     // 남은 시간

    // 현재 이벤트로 보낸 감소율(중복 이벤트 방지용)
    private float lastReduction = -1f;
    private Animator animator;
    private void Start()
    {
        animator = GetComponent<Animator>();
        timeCounter = runtimeStats.timer;

        // 게임 처음 시작 시 상태 알림 (예: 앱 재시작 대비)
        RaiseCookTimeReductionEvent();
    }

    private void Update()
    {
        
        timeCounter -= Time.deltaTime;
        if (timeCounter <= 0f)
        {
            isWorking = !isWorking;
            timeCounter = isWorking ? runtimeStats.timer : runtimeStats.cooltime;
            if (animator != null)
                animator.SetBool("Work", isWorking);
            // 근무 상태 변경 때마다 감소율 이벤트 발송
            RaiseCookTimeReductionEvent();
        }
    }

    private void RaiseCookTimeReductionEvent()
    {
        float reduction = isWorking ? cookTimeReduction : 0f;

        // 불필요한 중복 이벤트 방지
        if (Mathf.Approximately(reduction, lastReduction))
            return;

        lastReduction = reduction;

        EventBus<CookTimeReductionEvent>.Raise(new CookTimeReductionEvent(reduction));
    }

    public float GetCookTimeReduction()
    {
        return isWorking ? cookTimeReduction : 0f;
    }

    public bool IsWorking()
    {
        return isWorking;
    }
}


