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
            SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click, false);

            EventBus<ManagementActiveHandler>.Raise(new ManagementActiveHandler(isActive,ClickType.FoodSlot));
            isActive = false;
            EventBus<FoodSlotsSpawnHandler>.Raise(new FoodSlotsSpawnHandler());
            // °­È­µµ off
            EventBus<EnhanceFoodUIActiveHandler>.Raise(new EnhanceFoodUIActiveHandler(isActive));
    //        EventBus<ButtonHandler>.Raise(new ButtonHandler(isActive));
        }
        else
        {
            SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click, false);
            EventBus<FoodSlotsDeleteHandler>.Raise(new FoodSlotsDeleteHandler());
            EventBus<ManagementActiveHandler>.Raise(new ManagementActiveHandler(isActive, ClickType.FoodSlot));
            EventBus<ManagementActiveHandler>.Raise(new ManagementActiveHandler(isActive, ClickType.FoodAmount));

            
            isActive = true;

   //         EventBus<ButtonHandler>.Raise(new ButtonHandler(isActive));
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


    public bool isActive { get; set; } = true;

}
