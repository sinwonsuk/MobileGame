using UnityEngine;

public struct SitTableHandler : IEvent
{
    public SitTableHandler(Customer customer)
    {
        this.customer = customer;
    }

    public Customer customer { get; set; }
}
