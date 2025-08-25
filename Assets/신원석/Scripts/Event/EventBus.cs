using System;
using UnityEngine;

public static class EventBus<T> where T : IEvent
{
    public static event Action<T> OnEvent;
    public static void Raise(T evt) => OnEvent?.Invoke(evt);

    public static void Clear()
    {
        // 모든 구독 해제
        OnEvent = null;
    }
}

