using UnityEngine;

public struct FoodIncreaseHandler : IEvent
{
    public FoodIncreaseHandler(string name, int Setquantity)
    {
        this.foodname = name;
        this.Setquantity = Setquantity;
    }
    public string foodname { get; set; }
    public int Setquantity { get; set; }
}
