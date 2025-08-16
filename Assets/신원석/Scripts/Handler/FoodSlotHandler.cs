using UnityEngine;

public struct FoodSlotHandler : IEvent
{
    public FoodSlotHandler(string Image,string name,string Manual,string price)
    {
        this.Image = Image;
        this.name = name;
        this.Manual = Manual;
        this.price = price;
    }

    public string Image { get; set; }
    public string name { get; set; }

    public string Manual { get; set; }

    public string price { get; set; }
}
