using UnityEngine;
using System.Collections;

public class RestaurantStaff_Two : StaffBase, ICooldownReadable, IRemainingTimeReadable
{
    [Header("직원 데이터 (SO)")]
    public StaffStatsSO stats;
    public RuntimeStaffStatsSO runtimeStats;

    [Header("요리 시간 감소율 (0.2 = 20%)")]
    public float cookTimeReduction = 0.9f; // Inspector에서 조절

    private bool isWorking = false;  // 근무 중 여부
    private double timeCounter;      // 남은 시간

    // 현재 이벤트로 보낸 감소율(중복 이벤트 방지용)
    private float lastReduction = -1f;
    private Animator animator;

    private Coroutine heartbeatCo;   // ★ 하트비트용 코루틴 핸들

    private void OnEnable()
    {
        StartCoroutine(EmitStickyReductionNextFrame());
        StartHeartbeat(); // ★ 활성화되면 하트비트 시작
    }
    private void OnDisable()
    {
        StopHeartbeat();  // ★ 비활성화 시 중단
    }

    private IEnumerator EmitStickyReductionNextFrame()
    {
        yield return null;
        RaiseCookTimeReductionEvent(); // 상태 반영 1회
    }

    private void Start()
    {
        animator = GetComponent<Animator>();

        isWorking = false;                        // ★ 휴식부터
        timeCounter = runtimeStats.cooltime;      // ★ 쿨타임부터
        if (animator != null) animator.SetBool("Work", isWorking);

        RaiseCookTimeReductionEvent();            // ★ 초기 1회 알림
    }

    private void Update()
    {
        timeCounter -= Time.deltaTime;
        if (timeCounter <= 0f)
        {
            isWorking = !isWorking;
            timeCounter = isWorking ? runtimeStats.timer : runtimeStats.cooltime;
            if (animator != null) animator.SetBool("Work", isWorking);

            // 상태 바뀔 때마다 현재 감소율 통지(중복 방지 적용)
            RaiseCookTimeReductionEvent();
        }
    }

    // ========================= 핵심 변경부 =========================
    // 하트비트: 일정 주기마다 현재 감소율을 강제로 재발행
    private void StartHeartbeat()
    {
        if (heartbeatCo == null) heartbeatCo = StartCoroutine(HeartbeatLoop());
    }
    private void StopHeartbeat()
    {
        if (heartbeatCo != null)
        {
            StopCoroutine(heartbeatCo);
            heartbeatCo = null;
        }
    }
    private IEnumerator HeartbeatLoop()
    {
        // 0.5~1초 간격 추천: 너무 잦지 않게, 늦게 켜진 Cook도 금방 동기화
        const float interval = 0.8f;
        while (true)
        {
            // 중복검사 무시하고 강제 발행
            RaiseCookTimeReductionEvent(force: true);
            yield return new WaitForSeconds(interval);
        }
    }
    // =============================================================

    private void RaiseCookTimeReductionEvent(bool force = false)
    {
        float reduction = isWorking ? cookTimeReduction : 0f;
        if (!force && Mathf.Approximately(reduction, lastReduction)) return; // 중복 방지
        lastReduction = reduction;
        EventBus<CookTimeReductionEvent>.Raise(new CookTimeReductionEvent(reduction));
    }

    public float GetCookTimeReduction() => isWorking ? cookTimeReduction : 0f;
    public bool IsWorking() => isWorking;

    public float GetCooldownRatio()
    {
        double max = IsWorking() ? runtimeStats.timer : runtimeStats.cooltime;
        if (max <= 0) return 0f;
        return Mathf.Clamp01((float)(timeCounter / max));
    }

    public float GetRemainingSeconds() => Mathf.Max(0f, (float)timeCounter);
}
