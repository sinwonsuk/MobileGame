using UnityEngine;

public class ShopUIEvent : IEvent
{
    public bool isShopOpen;

    public ShopUIEvent(bool isOpen)
    {
        isShopOpen = isOpen;
    }
}