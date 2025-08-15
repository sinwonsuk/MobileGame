using UnityEngine;

public class RestaurantShopUIButton : BaseButton
{
    public override void OnClick()
    {
        ButtonManager.buttonClick = ButtonClick.none;
        EventBus<ToggleRestaurantShopEvent>.Raise(new ToggleRestaurantShopEvent());
        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click, false);
    }

    public override void OnExit()
    {
        ButtonManager.buttonClick = ButtonClick.none;
        EventBus<ToggleRestaurantShopEvent>.Raise(new ToggleRestaurantShopEvent());
    }
}
