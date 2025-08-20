using UnityEngine;

public class LifecycleListener : MonoBehaviour
{
	private static bool _installed;

	void Awake()
	{
		if (_installed) { Destroy(gameObject); return; }
		_installed = true;
		DontDestroyOnLoad(gameObject);
	}

	void OnApplicationPause(bool pause)
	{
		if (pause)
		{
			Debug.Log("[Lifecycle] Pause ¡æ Save");
			AutoSaveManager.Instance?.AutoSaveAll();
		}
	}

	void OnApplicationFocus(bool hasFocus)
	{
		if (!hasFocus)
		{
			Debug.Log("[Lifecycle] Focus Lost ¡æ Save");
			AutoSaveManager.Instance?.AutoSaveAll();
		}
	}

	void OnApplicationQuit()
	{
		Debug.Log("[Lifecycle] Quit ¡æ Save");
		AutoSaveManager.Instance?.AutoSaveAll();
	}
}
