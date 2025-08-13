using UnityEngine;

public class DungeonUIButton : BaseButton
{
    public override void OnClick()
    {
        EventBus<ToggleDungeonUIEvent>.Raise(new ToggleDungeonUIEvent());
        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click, false);
    }

    public override void OnExit()
    {
        EventBus<ToggleDungeonUIEvent>.Raise(new ToggleDungeonUIEvent());
    }
}
