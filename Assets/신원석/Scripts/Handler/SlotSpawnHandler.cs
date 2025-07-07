using UnityEngine;
using UnityEngine.UI;

public struct SlotSpawnHandler : IEvent
{
    // Constructor must have a return type. Fixed by adding 'public' and specifying the struct name as the return type.
    public SlotSpawnHandler(GameObject slot, FoodData data)
    {
        this.Slot = slot;
        this.SlotName = data.displayName;

        foodData = data;
    }

    public GameObject Slot { get; set; }

    public FoodData foodData { get; set; }
    public string SlotName { get; set; }
}
