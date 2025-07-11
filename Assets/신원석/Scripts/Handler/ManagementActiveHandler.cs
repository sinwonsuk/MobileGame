using UnityEngine;

public struct ManagementActiveHandler : IEvent
{
    public ManagementActiveHandler(bool isActive, ClickType clickType)
    {
        this.isActive = isActive;
        this.clickType = clickType;
    }

    public bool isActive { get; set; }
    public ClickType clickType { get; set; }

}
public struct SetManagementActiveEvent : IEvent
{

}

public struct ManagementActiveCheckHandler : IEvent
{
    public ManagementActiveCheckHandler(ClickType clickType, CustomerManager customerManager)
    {
        this.clickType = clickType;
        this.customerManager = customerManager;
    }

    public ClickType clickType { get; set; }
    public CustomerManager customerManager { get; set; }
}