using UnityEngine;

public struct FoodSlotHandler : IEvent
{
    public FoodSlotHandler(string Image,string name,string Manual)
    {
        this.Image = Image;
        this.name = name;
        this.Manual = Manual;
    }

    public string Image { get; set; }
    public string name { get; set; }

    public string Manual { get; set; }
}
