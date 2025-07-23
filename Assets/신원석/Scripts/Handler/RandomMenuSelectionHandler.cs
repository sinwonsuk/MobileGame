using UnityEngine;

public struct RandomMenuSelectionHandler :IEvent
{
    public RandomMenuSelectionHandler(Customer customer)
    {

        this.customer = customer;
    }


    public Customer customer { get; set; }
}
