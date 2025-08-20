using UnityEngine;

public class SettingCanvasOnOff : BaseButton
{
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		//EventBus<ButtonHandler>.OnEvent += ManagementButtonisActive;
		//EventBus<ButtonisActiveHandler>.OnEvent += ManagementButtonisActive;
	}

	// Update is called once per frame
	void Update()
	{

	}

	public void SettingCanvasActive()
	{

		//SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click, false);
		//EventBus<EnhanceFoodUIActiveHandler>.Raise(new EnhanceFoodUIActiveHandler(isActive));
		//EventBus<EnhanceFoodSlotsSpawnHandler>.Raise(new EnhanceFoodSlotsSpawnHandler());

		//else
		//{
		//    SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click, false);
		//    EventBus<EnhanceFoodSlotsDeleteHandler>.Raise(new EnhanceFoodSlotsDeleteHandler());
		//    EventBus<EnhanceFoodUIActiveHandler>.Raise(new EnhanceFoodUIActiveHandler(isActive));
		//    isActive = true;

		//}
	}

	//public void ManagementButtonisActive(ButtonHandler buttonHandler)
	//{
	//	isActive = buttonHandler.isActive;
	//}

	//public void ManagementButtonisActive(ButtonisActiveHandler buttonHandler)
	//{
	//	gameObject.SetActive(buttonHandler.isActive);
	//}

	public override void OnClick()
	{
		SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click, false);
		EventBus<EnhanceFoodUIActiveHandler>.Raise(new EnhanceFoodUIActiveHandler(true));
		EventBus<EnhanceFoodSlotsSpawnHandler>.Raise(new EnhanceFoodSlotsSpawnHandler());
	}

	public override void OnExit()
	{
		EventBus<EnhanceFoodSlotsDeleteHandler>.Raise(new EnhanceFoodSlotsDeleteHandler());
		EventBus<EnhanceFoodUIActiveHandler>.Raise(new EnhanceFoodUIActiveHandler(false));
	}

	//bool isActive = true;


}
