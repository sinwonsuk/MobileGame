using UnityEngine;

public class EnhanceFoodActiveOnOff : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EventBus<ButtonHandler>.OnEvent += ManagementButtonisActive;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void EnhanceFoodActive()
    {
        if (isActive)
        {
            EventBus<EnhanceFoodUIActiveHandler>.Raise(new EnhanceFoodUIActiveHandler(isActive));
            EventBus<EnhanceFoodSlotsSpawnHandler>.Raise(new EnhanceFoodSlotsSpawnHandler());
            isActive = false;
        }
        else
        {
            EventBus<EnhanceFoodSlotsDeleteHandler>.Raise(new EnhanceFoodSlotsDeleteHandler());
            EventBus<EnhanceFoodUIActiveHandler>.Raise(new EnhanceFoodUIActiveHandler(isActive));
            isActive = true;
        }
    }

    public void ManagementButtonisActive(ButtonHandler buttonHandler)
    {
        isActive = buttonHandler.isActive;
    }

    bool isActive = true;


}
