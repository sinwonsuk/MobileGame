using UnityEngine;

public struct FoodSlotHandler : IEvent
{
    public FoodSlotHandler(string Image,string name)
    {
        this.Image = Image;
        this.name = name;
    }

    public string Image { get; set; }
    public string name { get; set; }
}
