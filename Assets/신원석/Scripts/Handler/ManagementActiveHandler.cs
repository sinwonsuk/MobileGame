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

