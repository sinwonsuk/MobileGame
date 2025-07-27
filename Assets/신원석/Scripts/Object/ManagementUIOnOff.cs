using UnityEngine;

public class ManagementUIOnOff : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EventBus<ButtonHandler>.OnEvent += ManagementOnOff;
        EventBus<ButtonisActiveHandler>.OnEvent += ManagementButtonisActive;
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
            isActive = true;
        }
    }

    public void ManagementOnOff(ButtonHandler buttonHandler)
    {
        isActive = buttonHandler.isActive;
    }

    public void ManagementButtonisActive(ButtonisActiveHandler buttonHandler)
    {
        gameObject.SetActive(buttonHandler.isActive);
    }


    bool isActive = true;

}
