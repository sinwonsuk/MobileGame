using UnityEngine;

public struct GetCusomersEvent : IEvent
{
    public GetCusomersEvent(tetetetetetetetet Slot)
    {
        this.employee = Slot;
    }
    public tetetetetetetetet employee { get; set; }
}
public struct GetFirstCookEvent : IEvent
{
    public GetFirstCookEvent(tetetetetetetetet Slot)
    {
        this.employee = Slot;
        this.cooker = null; 
    }

    public GetFirstCookEvent(Cooker cooker)
    {
        this.cooker = cooker;
        this.employee = null;
    }

    public tetetetetetetetet employee { get; set; }
    public Cooker cooker { get; set; }
}
