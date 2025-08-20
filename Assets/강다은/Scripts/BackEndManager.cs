using BackEnd;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackendManager : MonoBehaviour
{
	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(this.gameObject);
		}
		else
		{
			Destroy(this.gameObject);
		}
	}

	void Start()
	{
		var bro = Backend.Initialize();

		if (bro.IsSuccess())
		{
			Debug.Log("초기화 성공 : " + bro);
			if (AutoSaveManager.Instance == null)
			{
				Instantiate(autoSaveManagerPrefab);
			}

			Backend.ErrorHandler.OnOtherDeviceLoginDetectedError = () =>
			{
				Debug.Log("다른 기기에서 로그인 감지됨");
				AutoSaveManager.Instance?.OnLoggedOut();

				// 유저 알림
				PopupManager.Show("다른 기기에서 로그인되어 접속이 종료되었습니다.", () =>
				{
					SceneManager.LoadScene("DaniTest");
				});
			};
		}
			
		else
			Debug.LogError("초기화 실패 : " + bro);
	}

	void OnApplicationQuit()
	{
		Debug.Log("종료 시 유저 데이터 저장 요청 완료");
		AutoSaveManager.Instance?.AutoSaveAll(true);
	}

	void OnApplicationPause(bool pause)
	{
		if (pause)
		{
			Debug.Log("일시 정지 시 데이터 저장");
			AutoSaveManager.Instance?.AutoSaveAll(true);
		}
	}

	void OnApplicationFocus(bool hasFocus)
	{
		if (!hasFocus)
		{
			Debug.Log("포커스 잃을 시 데이터 저장");
			AutoSaveManager.Instance?.AutoSaveAll(true);
		}
	}


	public static BackendManager Instance;
	public GameObject autoSaveManagerPrefab;
}
