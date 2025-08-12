
using UnityEngine;

public class InventoryUIButton : BaseButton
{
    public override void OnClick()
    {
        EventBus<ToggleInventoryEvent>.Raise(new ToggleInventoryEvent());
        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click, false);
    }
    public override void OnExit()
    {
        // ButtonManager가 같은 버튼을 다시 눌렀을 때 닫히도록: 토글 한 번 더
        EventBus<ToggleInventoryEvent>.Raise(new ToggleInventoryEvent());
    }
}