using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ToggleRestaurantShop : MonoBehaviour
{
    public void ToggleSHop()
    {
        EventBus<ToggleRestaurantShopEvent>.Raise(new ToggleRestaurantShopEvent());
        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click, false);
    }
}
