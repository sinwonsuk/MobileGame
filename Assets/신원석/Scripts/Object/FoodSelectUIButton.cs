using UnityEngine;

public class FoodSelectUIButton : MonoBehaviour
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
        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click,false);
        EventBus<ManagementActiveHandler>.Raise(new ManagementActiveHandler(false,ClickType.FoodSlot));
        EventBus<ManagementActiveHandler>.Raise(new ManagementActiveHandler(false, ClickType.FoodAmount));
        EventBus<ButtonHandler>.Raise(new ButtonHandler(true));
    }

}
