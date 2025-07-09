using Unity.VisualScripting;
using UnityEngine;

public struct MenuReduceHandler : IEvent
{
    public MenuReduceHandler(MenuBoardSlot slot)
    {
        this.slot = slot;
    }

    public MenuBoardSlot slot { get; set; }

}
