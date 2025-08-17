using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
#endif

public class TitleScreenController : MonoBehaviour
{
	
	void Awake()
	{
		if (title) _titleBasePos = title.anchoredPosition;
		//SoundManager.GetInstance().PlayBgm(SoundManager.bgm.TitleBgm);
	}

	private void Start()
	{
        SoundManager.GetInstance().PlayBgm(SoundManager.bgm.TitleBgm);
    }


#if ENABLE_INPUT_SYSTEM
	void OnEnable()
	{
		EnhancedTouchSupport.Enable();   // 실제 터치
		TouchSimulation.Enable();        // 에디터에서 마우스로 터치 시뮬
	}

	void OnDisable()
	{
		TouchSimulation.Disable();
		EnhancedTouchSupport.Disable();

        var sm = SoundManager.GetInstance();
        if (sm != null) sm.Bgm_Stop();   // 내부에서 bgmSource null 체크가 또 들어있어야 안전
       // SoundManager.GetInstance().Bgm_Stop();
	}
#endif


	void Update()
	{
		float t = Time.unscaledTime;

		if (title)
		{
			float y = _titleBasePos.y + Mathf.Sin(t * titleSpeed) * titleAmplitude;
			title.anchoredPosition = new Vector2(_titleBasePos.x, y);
		}

		if (_loading)
			return;

		bool pressed = false;

#if ENABLE_INPUT_SYSTEM
		// 디바이스 터치 또는 마우스 클릭(에디터)
		if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
			pressed = true;
		else if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
			pressed = true;
#else
        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
            pressed = true;
#endif

		if (pressed)
		{
			_loading = true;
			LoadNextScene();
		}
	
	}

	void LoadNextScene()
	{
		SceneChange.Instance.LoadSceneAsync(SceneName.DaniTest);
	}

	[Header("Float Targets")]
	[SerializeField] RectTransform title;
	[SerializeField] RectTransform touch;

	[Header("Motion")]
	[SerializeField] float titleAmplitude = 12f;
	[SerializeField] float titleSpeed = 1.2f;

	Vector2 _titleBasePos;
	bool _loading;
}
