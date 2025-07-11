using UnityEngine;

public struct CookDeleteHandler : IEvent
{
    public CookDeleteHandler(Customer customer)
    {
        this.customer = customer;
    }

    public Customer customer { get; set; }
}

