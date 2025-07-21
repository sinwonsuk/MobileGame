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

				// 유저 알림
				PopupManager.Show("다른 기기에서 로그인되어 접속이 종료되었습니다.\n게임을 다시 시작해주세요.");

				// 로그인 씬으로 이동
				SceneManager.LoadScene("DaniTest");
			};
		}
			
		else
			Debug.LogError("초기화 실패 : " + bro);
	}

	void OnApplicationQuit()
	{
		Debug.Log("종료 시 유저 데이터 저장 요청 완료");
		BackendGameData.Instance.GameDataUpdate();
	}

	public static BackendManager Instance;
	public GameObject autoSaveManagerPrefab;
}
