using UnityEngine;

public struct ButtonHandler : IEvent
{
    public ButtonHandler(bool isActive)
    {
        this.isActive = isActive;
    }


    public bool isActive { get; set; }

}

public struct EnhanceFoodButtonHandler : IEvent
{
    public EnhanceFoodButtonHandler(bool isActive)
    {
        this.isActive = isActive;
    }

    public bool isActive { get; set; }

}
