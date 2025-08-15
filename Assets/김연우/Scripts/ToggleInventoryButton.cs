using UnityEngine;

public class InventoryUIButton : BaseButton
{
    public override void OnClick()
    {
        ButtonManager.buttonClick = ButtonClick.none;
        EventBus<ToggleInventoryEvent>.Raise(new ToggleInventoryEvent());
        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click, false);
    }

    public override void OnExit()
    {
        ButtonManager.buttonClick = ButtonClick.none;
        EventBus<ToggleInventoryEvent>.Raise(new ToggleInventoryEvent());
    }
}
