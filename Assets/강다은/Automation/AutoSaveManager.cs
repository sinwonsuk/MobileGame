using BackEnd;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoSaveManager : MonoBehaviour
{
	private void Awake()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;
		DontDestroyOnLoad(gameObject);
	}

	private bool IsSavingContextActive()
	{
		if (blockWhenLoggedOut)
		{
			if (!Backend.IsInitialized) return false;
			if (string.IsNullOrEmpty(Backend.UserInDate)) return false;
		}

		if (allowedScenes != null && allowedScenes.Length > 0)
		{
			string cur = SceneManager.GetActiveScene().name;
			for (int i = 0; i < allowedScenes.Length; i++)
				if (allowedScenes[i] == cur) return true;
			return false;
		}
		return true;
	}

	public void RegisterAutoSavable(IAutoSavable savable)
	{
		if (!autoSavables.Contains(savable))
		{
			autoSavables.Add(savable);
			lastSaveTimes[savable] = Time.unscaledTime - saveInterval;
		}
	}

	public void AutoSaveAll()
	{
		if (!IsSavingContextActive())
		{
			Debug.Log($"[자동 저장 스킵] scene={SceneManager.GetActiveScene().name}");
			return;
		}

		Debug.Log("자동 저장 시도");



		foreach (var savable in autoSavables)
		{
			float lastSaved = lastSaveTimes.ContainsKey(savable) ? lastSaveTimes[savable] : 0f;
			float timeSinceLastSave = Time.unscaledTime - lastSaved;

			if (timeSinceLastSave >= saveInterval)
			{
				savable.AutoSave();
				lastSaveTimes[savable] = Time.unscaledTime;
			}
			else
			{
				//Debug.Log($"[{savable}] 저장 건너뜀 (경과 {timeSinceLastSave:F1}s)");
			}
		}
	}

	public void AutoSaveAll(bool force = false)
	{
		if (force && Time.unscaledTime - _lastForceAt < forceCooldown) return;
		if (force) _lastForceAt = Time.unscaledTime;

		if (!IsSavingContextActive() && !force)
		{
			//Debug.Log($"[AutoSave] 스킵(컨텍스트 비활성)");
			return;
		}

		foreach (var savable in autoSavables)
		{
			float last = lastSaveTimes.TryGetValue(savable, out var t) ? t : -999f;
			if (force || Time.unscaledTime - last >= saveInterval)
			{
				savable.AutoSave();              //  각 매니저가 더티만 저장
				lastSaveTimes[savable] = Time.unscaledTime;
			}
		}
	}

	public void ForceFlushSoon(float delay = 0.25f)
	{
		if (_flushSoonCo != null) StopCoroutine(_flushSoonCo);
		_flushSoonCo = StartCoroutine(FlushSoonCo(delay));
	}
	IEnumerator FlushSoonCo(float d) { yield return new WaitForSecondsRealtime(d); AutoSaveAll(true); }

	public void ClearAll()
	{
		lastSaveTimes.Clear();
		autoSavables.Clear();
	}


	private void Start()
	{
		InvokeRepeating(nameof(AutoSaveAll), 30f, 300f);
	}

	public void OnLoggedOut()
	{
		Debug.Log("[AutoSave] 로그아웃 -> 등록 초기화");
		ClearAll();
	}


	public static AutoSaveManager Instance { get; private set; }

	private Dictionary<IAutoSavable, float> lastSaveTimes = new();
	private List<IAutoSavable> autoSavables = new();

	private const float saveInterval = 20f;


	[SerializeField] string[] allowedScenes = { "SampleScene" };
	[SerializeField] bool blockWhenLoggedOut = true;

	[SerializeField] float forceCooldown = 2f;
	float _lastForceAt = -999f;

	Coroutine _flushSoonCo;
}
