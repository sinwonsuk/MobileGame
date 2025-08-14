using UnityEngine;
using UnityEngine.UI;
public struct CloseRestaurantShopEvent : IEvent { }

[RequireComponent(typeof(Button))]
public class CloseRestaurantShopButton : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click, false);
            EventBus<CloseRestaurantShopEvent>.Raise(new CloseRestaurantShopEvent());
        });
    }
}
