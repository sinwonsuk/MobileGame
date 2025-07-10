using UnityEngine;

public class ManagementUIOnOff : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ManagementUIActive()
    {
        if( isActive)
        {
            EventBus<ManagementActiveHandler>.Raise(new ManagementActiveHandler(isActive,ClickType.FoodSlot));
            isActive = false;
        }
        else
        {
            EventBus<ManagementActiveHandler>.Raise(new ManagementActiveHandler(isActive, ClickType.FoodSlot));
            EventBus<ManagementActiveHandler>.Raise(new ManagementActiveHandler(isActive, ClickType.FoodAmount));
            EventBus<MenuBoardSlotSpawnHandler>.Raise(new MenuBoardSlotSpawnHandler());
            isActive = true;
        }
    }

    bool isActive =true;


}
