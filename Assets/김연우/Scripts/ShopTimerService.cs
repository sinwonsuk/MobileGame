using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShopTimerService : MonoBehaviour
{
    static ShopTimerService _instance;
    public static ShopTimerService Instance
    {
        get
        {
            if (_instance == null)
            {
                var go = new GameObject(nameof(ShopTimerService));
                _instance = go.AddComponent<ShopTimerService>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    // 타이머 정보를 담는 내부 클래스
    class TimerInfo
    {
        public ShopItemTimer owner;
        public DateTime endTime;
        public Action onComplete;
        public bool isDone;
    }

    private readonly List<TimerInfo> _timers = new List<TimerInfo>();

    public void RegisterTimer(ShopItemTimer owner, DateTime endTime, Action onComplete)
    {
        _timers.Add(new TimerInfo
        {
            owner = owner,
            endTime = endTime,
            onComplete = onComplete,
            isDone = false
        });
    }

    public TimeSpan GetRemaining(ShopItemTimer owner)
    {
        var t = _timers.FirstOrDefault(x => x.owner == owner && !x.isDone);
        if (t == null) return TimeSpan.Zero;
        return t.endTime - DateTime.Now;
    }

    private void Update()
    {
        var now = DateTime.Now;
        foreach (var t in _timers)
        {
            if (!t.isDone && now >= t.endTime)
            {
                t.isDone = true;
                t.onComplete?.Invoke();
            }
        }
        // (선택) 완료된 타이머는 원한다면 리스트에서 제거 가능합니다.
    }
}
