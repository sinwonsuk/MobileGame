using UnityEngine;
using System;

[CreateAssetMenu(fileName = "ExpeditionRuntime_", menuName = "Game/Expedition/Runtime")]
public class RuntimeExpeditionSO : ScriptableObject
{
    [Header("정적 SO 참조 (여기만 연결하면 ID 자동 동기화)")]
    public ExpeditionSO staticSO;

    [Header("매칭 키 (Static SO의 expeditionId와 동일)")]
    public string indate;

    [Header("상태")]
    public bool isRunning;
    public bool rewardClaimed;

    [Header("UTC(ISO8601)")]
    public string departUtcIso;   // 출발
    public string arriveUtcIso;   // 도착

	[NonSerialized]
	public bool isDirty = false;

	public DateTime DepartUtc => ParseUtc(departUtcIso);
    public DateTime ArriveUtc  => ParseUtc(arriveUtcIso);

    private void OnValidate()
    {
        SyncIdFromStatic();
    }
    public void SyncIdFromStatic()
    {
        if (staticSO == null) return;
        if (string.IsNullOrEmpty(staticSO.Indate)) return;

        indate = staticSO.Indate;
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
