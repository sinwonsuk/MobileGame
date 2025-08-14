using UnityEngine;
using UnityEngine.UI;
public struct CloseHunterShopEvent : IEvent { }

[RequireComponent(typeof(Button))]
public class CloseHunterShopButton : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click, false);
            EventBus<CloseHunterShopEvent>.Raise(new CloseHunterShopEvent());
        });
    }
}