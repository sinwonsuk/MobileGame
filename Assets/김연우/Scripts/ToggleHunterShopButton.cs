using UnityEngine;

public class HunterShopUIButton : BaseButton
{
    void Start()
    {
        gameObject.SetActive(false);
        EventBus<ButtonisActiveHandler>.OnEvent += ManagementButtonisActive;
    }

    private void OnDestroy()
    {
        EventBus<ButtonisActiveHandler>.OnEvent -= ManagementButtonisActive;
    }

    public override void OnClick()
    {
        ButtonManager.buttonClick = ButtonClick.none;
        EventBus<ToggleHunterShopEvent>.Raise(new ToggleHunterShopEvent(true));
        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click, false);
    }

    public void ManagementButtonisActive(ButtonisActiveHandler buttonHandler)
    {
        if(gameObject == null)
        {
            return;
        }

        gameObject.SetActive(!buttonHandler.isActive);
    }

    public override void OnExit()
    {
        ButtonManager.buttonClick = ButtonClick.none;
        EventBus<ToggleHunterShopEvent>.Raise(new ToggleHunterShopEvent(false));
    }
}
