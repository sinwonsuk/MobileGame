using UnityEngine;

public class ToggleDungeonUIEvent : IEvent 
{
    public ToggleDungeonUIEvent(bool check)
    {
        this.check = check;
    }
    public bool check { get; set; }


}

