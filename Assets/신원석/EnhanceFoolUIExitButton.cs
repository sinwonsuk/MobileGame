using UnityEngine;

public class EnhanceFoodSelectUIExitButton : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnClick()
    {
        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click, false);
        EventBus<EnhanceFoodUIActiveHandler>.Raise(new EnhanceFoodUIActiveHandler(false));
        EventBus<ButtonHandler>.Raise(new ButtonHandler(true));
    }
    public void Back()
    {
        EventBus<SetEnhanceFoodActiveEvent>.Raise(new SetEnhanceFoodActiveEvent());
    }
}
