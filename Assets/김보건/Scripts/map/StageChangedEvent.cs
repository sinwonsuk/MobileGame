using UnityEngine;

public class StageChangedEvent : IEvent
{
    public int stage;
    public bool isBoss;

    public StageChangedEvent(int stage, bool isBoss)
    {
        this.stage = stage;
        this.isBoss = isBoss;
    }
}
