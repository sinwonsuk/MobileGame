using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public class ExpeditionManager : MonoBehaviour
{
    public static ExpeditionManager Instance { get; private set; }

    [Header("정적 파견지 SO들 (예: 10개)")]
    public ExpeditionSO[] allExpeditions;

    [Header("런타임 파견지 SO들 (예: 10개)")]
    public RuntimeExpeditionSO[] allRuntimeExpeditions;

    // id -> (static, runtime)
    private readonly Dictionary<string, (ExpeditionSO stat, RuntimeExpeditionSO run)> pairs = new();

    public event Action<string> OnChanged; // 특정 id 변경 알림

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        BuildPairs();
    }

    private void OnValidate()
    {
        // 인스펙터에서 배열 교체/드래그 할 때도 자동 동기화
        BuildPairs();
    }

    private void BuildPairs()
    {
        pairs.Clear();
        if (allExpeditions == null || allRuntimeExpeditions == null) return;

        foreach (var run in allRuntimeExpeditions)
        {
            if (run == null) continue;
            run.SyncIdFromStatic();
        }
        var statById = allExpeditions
            ?.Where(s => s != null && !string.IsNullOrEmpty(s.Indate))
            .GroupBy(s => s.Indate).ToDictionary(g => g.Key, g => g.First());

        var runById = allRuntimeExpeditions
            ?.Where(r => r != null && !string.IsNullOrEmpty(r.Indate))
            .GroupBy(r => r.Indate).ToDictionary(g => g.Key, g => g.First());

        if (statById == null || runById == null) return;

        foreach (var id in statById.Keys)
        {
            if (!runById.ContainsKey(id))
            {
                Debug.LogWarning($"[ExpeditionManager] 런타임 SO 없음: {id}");
                continue;
            }
            pairs[id] = (statById[id], runById[id]);
        }

        foreach (var id in runById.Keys)
            if (!statById.ContainsKey(id))
                Debug.LogWarning($"[ExpeditionManager] 정적 SO 없음: {id}");
    }


    public bool CanStart(string id)
    {
        if (!pairs.TryGetValue(id, out var p)) return false;
        return !p.run.isRunning;
    }

    // 1) 시작: 항상 '미수령'으로 시작
    public bool StartExpedition(string id)
    {
        if (!pairs.TryGetValue(id, out var p)) return false;   // ← FindPair 대신 TryGetValue
        if (p.run.isRunning) return false;

        p.run.StartNowUtc(p.stat.durationHours);
        p.run.rewardClaimed = false;                            // 시작 시 보상 미수령
        OnChanged?.Invoke(id);
        return true;
    }


    public TimeSpan GetRemaining(string id)
    {
        if (!pairs.TryGetValue(id, out var p)) return TimeSpan.Zero;
        if (!p.run.isRunning) return TimeSpan.Zero;

        var rem = p.run.ArriveUtc - DateTime.UtcNow;
        return rem > TimeSpan.Zero ? rem : TimeSpan.Zero;
    }

    public bool IsDone(string id)
    {
        if (!pairs.TryGetValue(id, out var p)) return false;

        if (p.run.isRunning && DateTime.UtcNow >= p.run.ArriveUtc)
        {
            p.run.isRunning = false;

            TryClaimReward(id);

            return true; 
        }
        return !p.run.isRunning && !p.run.rewardClaimed && !string.IsNullOrEmpty(p.run.arriveUtcIso);
    }



    // 3) 보상 수령: 이때만 지급 + 초기화
    public bool TryClaimReward(string id)
    {
        if (!pairs.TryGetValue(id, out var p)) return false;

        if (p.run.isRunning || DateTime.UtcNow < p.run.ArriveUtc || p.run.rewardClaimed)
            return false;

        // ★ 여기서만 지급 ★
        if (p.stat.rewards != null)
        {
            foreach (var r in p.stat.rewards)
            {
                if (r?.ingredientData == null) continue;
                InventoryManager.Instance.AddItem(r.ingredientData.indate, Mathf.Max(0, r.amount));
            }
        }

        p.run.rewardClaimed = true;
        p.run.Clear();

        OnChanged?.Invoke(id);
        return true;
    }


    public float GetProgress01(string id)
    {
        if (!pairs.TryGetValue(id, out var p)) return 0f;
        if (!p.run.isRunning || p.stat.durationHours <= 0f) return 0f;

        double total = TimeSpan.FromHours(p.stat.durationHours).TotalSeconds;
        double elapsed = Math.Clamp((DateTime.UtcNow - p.run.DepartUtc).TotalSeconds, 0, total);
        return (float)(elapsed / total);
    }

    // 유틸: 전부 초기화/중단
    public void ResetAll()
    {
        foreach (var kv in pairs) kv.Value.run.Clear();
    }

    // 유틸: 런타임 SO 열거(외부 조회용)
    public IEnumerable<RuntimeExpeditionSO> EnumerateRuntime() => pairs.Values.Select(v => v.run);
}
