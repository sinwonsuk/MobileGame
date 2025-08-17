
public readonly struct ToggleHunterShopEvent : IEvent 
{ 
    public ToggleHunterShopEvent(bool check)
    {
        this.Check = check;
    }

    public bool Check { get; } 

}
public readonly struct ToggleRestaurantShopEvent : IEvent { }