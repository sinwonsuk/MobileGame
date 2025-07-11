using UnityEngine;

public struct FoodExplanationHandler : IEvent
{
    public FoodExplanationHandler(MenuManager menuManager,string name)
    {
        MenuManager = menuManager;
        Name = name;
    }

    public MenuManager MenuManager { get; set; }
    public string Name { get; set; }
}
