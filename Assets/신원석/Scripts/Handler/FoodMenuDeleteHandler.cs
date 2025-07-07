using UnityEngine;

public struct FoodMenuDeleteHandler :IEvent
{
    public FoodMenuDeleteHandler(string name)
    {
        this.foodname = name;
    }

    public string foodname { get; set; }

}
