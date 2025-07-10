using UnityEngine;

public struct CookMoveHandler : IEvent
{
    public CookMoveHandler(Transform transform, Customer customer)
    {
        this.TableTransform = transform;
        this.customer = customer;
    }

    public Transform TableTransform { get; set; }
    public Customer customer { get; set; }
}

public struct CookFillamountHandler : IEvent
{
    public CookFillamountHandler(FoodClick FoodClick)
    {
        this.FoodClick = FoodClick;
    }

    public FoodClick FoodClick { get; set; }
}
