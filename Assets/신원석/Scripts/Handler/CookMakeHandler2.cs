using UnityEngine;

public struct GetCusomersEvent : IEvent
{
    public GetCusomersEvent(RestaurantStaff_One Slot)
    {
        this.employee = Slot;
    }
    public RestaurantStaff_One employee { get; set; }
}
public struct GetFirstCookEvent : IEvent
{
    public GetFirstCookEvent(RestaurantStaff_One Slot)
    {
        this.employee = Slot;
        this.cooker = null; 
    }

    public GetFirstCookEvent(Cooker cooker)
    {
        this.cooker = cooker;
        this.employee = null;
    }

    public RestaurantStaff_One employee { get; set; }
    public Cooker cooker { get; set; }
}
