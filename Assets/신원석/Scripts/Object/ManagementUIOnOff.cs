using UnityEngine;

public class ManagementUIOnOff : BaseButton
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EventBus<ButtonHandler>.OnEvent += ManagementOnOff;
        EventBus<ButtonisActiveHandler>.OnEvent += ManagementButtonisActive;
    }

    private void OnDisable()
    {
        EventBus<ButtonHandler>.OnEvent -= ManagementOnOff;
        EventBus<ButtonisActiveHandler>.OnEvent -= ManagementButtonisActive;
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void ManagementUIActive()
    {

           

       //// °­È­ ²ô±â 
       // EventBus<EnhanceFoodUIActiveHandler>.Raise(new EnhanceFoodUIActiveHandler(false));        
    }

    public void ManagementOnOff(ButtonHandler buttonHandler)
    {
        isActive = buttonHandler.isActive;
    }

    public void ManagementButtonisActive(ButtonisActiveHandler buttonHandler)
    {
        gameObject.SetActive(buttonHandler.isActive);
    }

    public override void OnClick()
    {
        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click, false);
        EventBus<ManagementActiveHandler>.Raise(new ManagementActiveHandler(isActive, ClickType.FoodSlot));
        EventBus<FoodSlotsSpawnHandler>.Raise(new FoodSlotsSpawnHandler());
    }

    public override void OnExit()
    {
        EventBus<FoodSlotsDeleteHandler>.Raise(new FoodSlotsDeleteHandler());
        EventBus<ManagementActiveHandler>.Raise(new ManagementActiveHandler(false, ClickType.FoodSlot));
        EventBus<ManagementActiveHandler>.Raise(new ManagementActiveHandler(false, ClickType.FoodAmount));
    }

    public bool isActive { get; set; } = true;

}
