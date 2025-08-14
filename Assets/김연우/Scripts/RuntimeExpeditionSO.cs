using UnityEngine;
using System;

[CreateAssetMenu(fileName = "ExpeditionRuntime_", menuName = "Game/Expedition/Runtime")]
public class RuntimeExpeditionSO : ScriptableObject
{
    [Header("정적 SO 참조 (여기만 연결하면 ID 자동 동기화)")]
    public ExpeditionSO staticSO;

    [Header("매칭 키 (Static SO의 expeditionId와 동일)")]
    public string Indate;

    [Header("상태")]
    public bool isRunning;
    public bool rewardClaimed;

    [Header("UTC(ISO8601)")]
    public string departUtcIso;   // 출발
    public string arriveUtcIso;   // 도착

    public DateTime DepartUtc => ParseUtc(departUtcIso);
    public DateTime ArriveUtc  => ParseUtc(arriveUtcIso);

    // ---- 에디터에서 값이 바뀔 때/드래그 했을 때 자동 동기화 ----
    private void OnValidate()
    {
        SyncIdFromStatic();
    }

    // 매니저/런타임에서도 호출 가능 (안전)
    public void SyncIdFromStatic()
    {
        if (staticSO == null) return;
        if (string.IsNullOrEmpty(staticSO.Indate)) return;

        // 정적 SO의 ID를 그대로 복사
        Indate = staticSO.Indate;
    }

    public void StartNowUtc(float durationHours, DateTime? nowOverride = null)
    {
        var now = nowOverride ?? DateTime.UtcNow;
        isRunning     = true;
        rewardClaimed = false;
        departUtcIso  = now.ToString("O");
        arriveUtcIso  = now.AddHours(Mathf.Max(0.01f, durationHours)).ToString("O");
    }

    public void Clear()
    {
        isRunning     = false;
        rewardClaimed = false;
        departUtcIso  = null;
        arriveUtcIso  = null;
    }

    public static DateTime ParseUtc(string iso)
    {
        if (string.IsNullOrEmpty(iso)) return DateTime.MinValue;
        if (DateTime.TryParse(iso, null,
            System.Globalization.DateTimeStyles.RoundtripKind, out var t))
            return t.Kind == DateTimeKind.Utc ? t : t.ToUniversalTime();
        return DateTime.MinValue;
    }
}
