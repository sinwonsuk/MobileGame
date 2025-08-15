using UnityEngine;

public class MenuUIOnOff : BaseButton
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {       
        EventBus<ButtonisActiveHandler>.OnEvent += ManagementButtonisActive;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IsActive()
    {

    }

    public void ManagementButtonisActive(ButtonisActiveHandler buttonHandler)
    {
        gameObject.SetActive(buttonHandler.isActive);
    }

    public override void OnClick()
    {
        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click, false);
        EventBus<MenuBoardActiveHandler>.Raise(new MenuBoardActiveHandler(true));
    }

    public override void OnExit()
    {
        EventBus<MenuBoardActiveHandler>.Raise(new MenuBoardActiveHandler(false));
    }

    private bool Check = false;

}
