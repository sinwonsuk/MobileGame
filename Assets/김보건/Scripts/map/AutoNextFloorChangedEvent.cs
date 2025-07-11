using UnityEngine;

public class AutoNextFloorChangedEvent : IEvent
{
    public bool isAutoNext;

    public AutoNextFloorChangedEvent(bool isAutoNext)
    {
        this.isAutoNext = isAutoNext;
    }
}