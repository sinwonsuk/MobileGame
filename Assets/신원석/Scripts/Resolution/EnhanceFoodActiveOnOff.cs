using UnityEngine;

public class EnhanceFoodActiveOnOff : BaseButton
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EventBus<ButtonHandler>.OnEvent += ManagementButtonisActive;
        EventBus<ButtonisActiveHandler>.OnEvent += ManagementButtonisActive;
    }


    private void OnDestroy()
    {
        EventBus<ButtonHandler>.OnEvent -= ManagementButtonisActive;
        EventBus<ButtonisActiveHandler>.OnEvent -= ManagementButtonisActive;
    }


    // Update is called once per frame
    void Update()
    {

    }

    public void EnhanceFoodActive()
    {

    }

    public void ManagementButtonisActive(ButtonHandler buttonHandler)
    {
        isActive = buttonHandler.isActive;
    }

    public void ManagementButtonisActive(ButtonisActiveHandler buttonHandler)
    {
        if (gameObject == null)
        {
            return;
        }
        gameObject.SetActive(buttonHandler.isActive);
    }

    public override void OnClick()
    {
        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click, false);
        EventBus<EnhanceFoodUIActiveHandler>.Raise(new EnhanceFoodUIActiveHandler(true));
        EventBus<EnhanceFoodSlotsDeleteHandler>.Raise(new EnhanceFoodSlotsDeleteHandler());
        EventBus<EnhanceFoodSlotsSpawnHandler>.Raise(new EnhanceFoodSlotsSpawnHandler());

        if (!tutorialBool.Instance.clearLevelUpTuto)
        {
            TutorialManager.Instance?.StartTutorial(TutorialManager.TutorialType.FoodLevelUp);
        }
    }

    public override void OnExit()
    {
        EventBus<EnhanceFoodSlotsDeleteHandler>.Raise(new EnhanceFoodSlotsDeleteHandler());
        EventBus<EnhanceFoodUIActiveHandler>.Raise(new EnhanceFoodUIActiveHandler(false));
    }

    bool isActive = true;


}
