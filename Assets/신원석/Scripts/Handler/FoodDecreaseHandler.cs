using UnityEngine;

public struct FoodDecreaseHandler : IEvent
{
    public FoodDecreaseHandler(string name,int Setquantity)
    {
        this.foodname = name;
        this.Setquantity = Setquantity;

    }
    public string foodname { get; set; }
    public int Setquantity { get; set; }
}
