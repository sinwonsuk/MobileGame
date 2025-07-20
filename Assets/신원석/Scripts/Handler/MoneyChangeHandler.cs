using UnityEngine;

public struct MoneyChangeMusHandler : IEvent
{
    public MoneyChangeMusHandler(int money)
    {
        this.money = money;
    }

    public int money { get; set; }
}
public struct MoneyChangePusHandler : IEvent
{
    public MoneyChangePusHandler(int money)
    {
        this.money = money;
    }

    public int money { get; set; }
}
