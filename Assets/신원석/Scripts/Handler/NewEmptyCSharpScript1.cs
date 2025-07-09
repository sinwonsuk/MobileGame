using UnityEngine;

public struct SetMenuParentTransformHandler :IEvent
{
    public SetMenuParentTransformHandler(FoodAmountController controller)
    {
        Controller = controller;
    }

    public FoodAmountController Controller { get; set; }


}
