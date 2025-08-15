using UnityEngine;

public struct LocationChangedEvent : IEvent
{
    public location value;
    public LocationChangedEvent(location v) { value = v; }
}
public static class LocationState
{
    public static location Current = location.none;
}