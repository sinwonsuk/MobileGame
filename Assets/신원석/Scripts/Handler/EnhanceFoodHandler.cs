using UnityEngine;

public struct SetEnhanceFoodActiveEvent : IEvent{ }
public struct EnhanceFoodSlotsSpawnHandler : IEvent { }
public struct EnhanceFoodUIActiveHandler : IEvent
{
    public EnhanceFoodUIActiveHandler(bool isActive)
    {
        this.isActive = isActive;
    }

    public bool isActive { get; set; }
}


public struct EnhanceFoodSlotSpawnHandler : IEvent
{
    // Constructor must have a return type. Fixed by adding 'public' and specifying the struct name as the return type.
    public EnhanceFoodSlotSpawnHandler(GameObject slot, FoodData data)
    {
        this.Slot = slot;
        this.SlotName = data.displayName;
        this.Image = data.foodSprite;
        this.Level = data.Level;
        this.probability = data.enhanceSteps[data.Level].successRate;

        foodData = data;
    }

    public GameObject Slot { get; set; }

    public FoodData foodData { get; set; }
    public string SlotName { get; set; }

    public string Image { get; set; }

    public int Level { get; set; }

    public float probability { get; set; }


}



public struct EnhanceFoodSlotHandler : IEvent
{
    public EnhanceFoodSlotHandler(FoodData foodData,string Image, string name, int CurrentLevel,int CurrentPrice,int FutureLevel,int FuturePrice, int maxLevel,float probability)
    {
        this.Image = Image;
        this.name = name;
        this.CurrentLevel = CurrentLevel;
        this.CurrentPrice = CurrentPrice;
        this.FutureLevel = FutureLevel;
        this.FuturePrice = FuturePrice;
        this.maxLevel = maxLevel;
        this.foodData = foodData;
        this.CurrentProbability = probability;
    }

    public string Image { get; set;}
    public string name { get; set;}
    public int CurrentLevel { get; set; }
    public int CurrentPrice { get; set; }
    public int FutureLevel { get; set; }
    public int FuturePrice { get; set; }
    public int maxLevel { get; set; }

    public float CurrentProbability { get; set; }

    public FoodData foodData { get; set; }

}


public struct EnhanceFoodSlotDeleteHandler : IEvent{ }

public struct EnhanceFoodSlotsDeleteHandler : IEvent{ }