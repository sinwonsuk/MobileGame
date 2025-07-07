using UnityEngine;

public struct FoodSlotHandler : IEvent
{
    public FoodSlotHandler(string Image)
    {
        this.Image = Image;       
    }

    public string Image { get; set; }
}
