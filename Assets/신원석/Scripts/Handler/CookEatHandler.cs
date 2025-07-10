public struct CustomerStateChangeHandler : IEvent
{
    public CustomerStateChangeHandler(CustomerState customerState, Customer customer)
    {
        this.customerState = customerState;
        this.customer = customer;
    }

    public CustomerState customerState { get; set; }
    public Customer customer { get; set; }
}
