using UnityEngine;
using UnityEngine.UI;
public struct CloseInventoryEvent : IEvent { }

[RequireComponent(typeof(Button))]
public class CloseInventoryButton : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click, false);
            EventBus<CloseInventoryEvent>.Raise(new CloseInventoryEvent());
        });
    }
}
