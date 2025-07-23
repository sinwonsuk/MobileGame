using UnityEngine;

public class DungeonSlideToggleEvent : IEvent
{
    public bool isDungeonActive;

    public DungeonSlideToggleEvent(bool isDungeonActive)
    {
        this.isDungeonActive = isDungeonActive;
    }
}