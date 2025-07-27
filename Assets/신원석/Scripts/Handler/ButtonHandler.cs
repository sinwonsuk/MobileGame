using UnityEngine;

public struct ButtonHandler : IEvent
{
    public ButtonHandler(bool isActive)
    {
        this.isActive = isActive;
    }


    public bool isActive { get; set; }

}

public struct ButtonisActiveHandler : IEvent
{
    public ButtonisActiveHandler(bool isActive)
    {
        this.isActive = isActive;
    }

    public bool isActive { get; set; }

}
