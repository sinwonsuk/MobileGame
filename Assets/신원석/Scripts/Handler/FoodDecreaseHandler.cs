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

public struct EnhanceFoodDecreaseHandler : IEvent
{
    public EnhanceFoodDecreaseHandler(string foodName,string ingredientName, int Setquantity)
    {
        this.foodName = foodName;
        this.ingredientName = ingredientName;
        this.Setquantity = Setquantity;

    }
    public string foodName { get; set; }
    public string ingredientName { get; set; }
    public int Setquantity { get; set; }
}

