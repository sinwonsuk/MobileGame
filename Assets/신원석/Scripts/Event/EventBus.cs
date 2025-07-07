using System;
using UnityEngine;

public static class EventBus<T> where T : IEvent
{
    public static event Action<T> OnEvent;
    public static void Raise(T evt) => OnEvent?.Invoke(evt);
}

