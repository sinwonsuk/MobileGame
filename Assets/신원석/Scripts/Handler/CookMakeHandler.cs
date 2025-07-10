using UnityEngine;

public struct CookMakeHandler : IEvent
{
    public CookMakeHandler(string name, MenuBoardSlot slot)
    {
        foodname = name;
        Slot = slot;
    }
    public string foodname { get; set; }
    public MenuBoardSlot Slot { get; set; }
}
