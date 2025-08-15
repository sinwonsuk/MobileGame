using UnityEngine;

public class HunterShopUIButton : BaseButton
{
    void Start()
    {
        gameObject.SetActive(false);
        EventBus<ButtonisActiveHandler>.OnEvent += ManagementButtonisActive;
    }

    public override void OnClick()
    {
        ButtonManager.buttonClick = ButtonClick.none;
        EventBus<ToggleHunterShopEvent>.Raise(new ToggleHunterShopEvent());
        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click, false);
    }

    public void ManagementButtonisActive(ButtonisActiveHandler buttonHandler)
    {
        gameObject.SetActive(!buttonHandler.isActive);
    }

    public override void OnExit()
    {
        ButtonManager.buttonClick = ButtonClick.none;
        EventBus<ToggleHunterShopEvent>.Raise(new ToggleHunterShopEvent());
    }
}
