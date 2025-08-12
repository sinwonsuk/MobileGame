using UnityEngine;

public class RestaurantShopUIButton : BaseButton
{
    public override void OnClick()
    {
        EventBus<ToggleRestaurantShopEvent>.Raise(new ToggleRestaurantShopEvent());
        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click, false);
    }
    public override void OnExit()
    {
        EventBus<ToggleRestaurantShopEvent>.Raise(new ToggleRestaurantShopEvent());
    }
}