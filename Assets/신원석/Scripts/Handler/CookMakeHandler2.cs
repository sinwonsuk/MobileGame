using UnityEngine;

public struct GetCusomersEvent : IEvent
{
    public GetCusomersEvent(RestaurantStaff_Two Slot)
    {
        this.employee = Slot;
    }
    public RestaurantStaff_Two employee { get; set; }
}
public struct GetFirstCookEvent : IEvent
{
    public GetFirstCookEvent(RestaurantStaff_Two Slot)
    {
        this.employee = Slot;
        this.cooker = null; 
    }

    public GetFirstCookEvent(Cooker cooker)
    {
        this.cooker = cooker;
        this.employee = null;
    }

    public RestaurantStaff_Two employee { get; set; }
    public Cooker cooker { get; set; }
}
