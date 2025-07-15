using UnityEngine;

public class FoodQuantityChangedEvent : IEvent
{
    public FoodQuantityChangedEvent(aaaa Slot)
    {
        this.Inven = Slot;
    }
    public aaaa Inven { get; set; }
}
