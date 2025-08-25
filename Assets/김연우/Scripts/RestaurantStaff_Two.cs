using UnityEngine;

public class RestaurantStaff_Two : StaffBase, ICooldownReadable, IRemainingTimeReadable
{
    [Header("직원 데이터 (SO)")]
    public StaffStatsSO stats;
    public RuntimeStaffStatsSO runtimeStats;

    [Header("요리 시간 감소율 (0.2 = 20%)")]
    public float cookTimeReduction = 0.9f; // Inspector에서 조절

    private bool isWorking = false;  // 근무 중 여부
    private double timeCounter;     // 남은 시간

    // 현재 이벤트로 보낸 감소율(중복 이벤트 방지용)
    private float lastReduction = -1f;
    private Animator animator;

    private void OnEnable()
    {
        StartCoroutine(EmitStickyReductionNextFrame());
    }
    private System.Collections.IEnumerator EmitStickyReductionNextFrame()
    {
        yield return null; 
        RaiseCookTimeReductionEvent(); 
    }

    private void Start()
    {
        animator = GetComponent<Animator>();

        isWorking = false;                       // ★ 휴식부터
        timeCounter = runtimeStats.cooltime;     // ★ 쿨타임부터
        if (animator != null) animator.SetBool("Work", isWorking);

        RaiseCookTimeReductionEvent();
    }

    private void Update()
    {
        timeCounter -= Time.deltaTime;
        if (timeCounter <= 0f)
        {
            isWorking = !isWorking;
            timeCounter = isWorking ? runtimeStats.timer : runtimeStats.cooltime;
            if (animator != null) animator.SetBool("Work", isWorking);

            // 상태 바뀔 때마다 현재 감소율 통지
            RaiseCookTimeReductionEvent();
        }
    }


    private void RaiseCookTimeReductionEvent()
    {
        float reduction = isWorking ? cookTimeReduction : 0f;
        if (Mathf.Approximately(reduction, lastReduction)) return; // 중복 방지
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
    public float GetCooldownRatio()
    {
        double max = IsWorking() ? runtimeStats.timer : runtimeStats.cooltime;
        if (max <= 0) return 0f;
        return Mathf.Clamp01((float)(timeCounter / max));
    }

    public float GetRemainingSeconds()
    {
        return Mathf.Max(0f, (float)timeCounter);
    }
}


