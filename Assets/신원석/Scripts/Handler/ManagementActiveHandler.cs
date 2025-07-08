using UnityEngine;

public struct ManagementActiveHandler : IEvent
{
    public ManagementActiveHandler(bool isActive)
    {
        this.isActive = isActive;
    }

    public bool isActive { get; set; }

}
public struct SetManagementActiveEvent : IEvent
{

}
