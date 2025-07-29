using UnityEngine;

public class EnhanceFoodActiveOnOff : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EventBus<ButtonHandler>.OnEvent += ManagementButtonisActive;
        EventBus<ButtonisActiveHandler>.OnEvent += ManagementButtonisActive;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void EnhanceFoodActive()
    {
        if (isActive)
        {
            SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click, false);
            EventBus<EnhanceFoodUIActiveHandler>.Raise(new EnhanceFoodUIActiveHandler(isActive));
            EventBus<EnhanceFoodSlotsSpawnHandler>.Raise(new EnhanceFoodSlotsSpawnHandler());


            isActive = false;

            // ¸Å´ºµµ off 
            EventBus<ManagementActiveHandler>.Raise(new ManagementActiveHandler(isActive, ClickType.FoodSlot));
            EventBus<ManagementActiveHandler>.Raise(new ManagementActiveHandler(isActive, ClickType.FoodAmount));

        }

        else
        {
            SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click, false);
            EventBus<EnhanceFoodSlotsDeleteHandler>.Raise(new EnhanceFoodSlotsDeleteHandler());
            EventBus<EnhanceFoodUIActiveHandler>.Raise(new EnhanceFoodUIActiveHandler(isActive));
            isActive = true;

        }
    }

    public void ManagementButtonisActive(ButtonHandler buttonHandler)
    {
        isActive = buttonHandler.isActive;
    }

    public void ManagementButtonisActive(ButtonisActiveHandler buttonHandler)
    {
        gameObject.SetActive(buttonHandler.isActive);
    }


    bool isActive = true;


}
