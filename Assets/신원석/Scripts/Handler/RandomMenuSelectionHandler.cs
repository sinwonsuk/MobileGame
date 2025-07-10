using UnityEngine;

public struct RandomMenuSelectionHandler :IEvent
{
    public RandomMenuSelectionHandler(CustomerManager customerManager)
    {
        this.CustomerManager = customerManager;
    }

    public CustomerManager CustomerManager { get; set; }

}
