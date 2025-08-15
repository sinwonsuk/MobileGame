using UnityEngine;

public class RestaurantShopUIButton : BaseButton
{
    void Start()
    {
        EventBus<ButtonisActiveHandler>.OnEvent += ManagementButtonisActive;
    }

    private void OnDestroy()
    {
        EventBus<ButtonisActiveHandler>.OnEvent -= ManagementButtonisActive;
    }

    public override void OnClick()
    {
        ButtonManager.buttonClick = ButtonClick.none;
        EventBus<ToggleRestaurantShopEvent>.Raise(new ToggleRestaurantShopEvent());
        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click, false);
    }
    public void ManagementButtonisActive(ButtonisActiveHandler buttonHandler)
    {
                if(gameObject == null)
        {
            return;
        }
        gameObject.SetActive(buttonHandler.isActive);
    }
    public override void OnExit()
    {
        ButtonManager.buttonClick = ButtonClick.none;
        EventBus<ToggleRestaurantShopEvent>.Raise(new ToggleRestaurantShopEvent());
    }
}
