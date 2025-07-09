public struct MenuLoadedEvent : IEvent
{
    public MenuLoadedEvent(CustomerManager customerManager)
    {
        CustomerManager = customerManager;
    }

    public CustomerManager CustomerManager { get; set; }
}