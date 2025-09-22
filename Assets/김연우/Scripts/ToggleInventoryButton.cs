using UnityEngine;

public class InventoryUIButton : BaseButton
{
    public override void OnClick()
    {
        ButtonManager.buttonClick = ButtonClick.none;
        EventBus<ToggleInventoryEvent>.Raise(new ToggleInventoryEvent());
        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click, false);

        if (!tutorialBool.Instance.clearInvenTuto)
            TutorialManager.Instance?.StartTutorial(TutorialManager.TutorialType.Inventory);
    }

    public override void OnExit()
    {
        ButtonManager.buttonClick = ButtonClick.none;
        EventBus<ToggleInventoryEvent>.Raise(new ToggleInventoryEvent());
    }
}
