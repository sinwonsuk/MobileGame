using BackEnd;
using UnityEngine;

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
