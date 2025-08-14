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

		if (SceneManager.GetActiveScene().name != "SampleScene")
		{
			Debug.Log($"[자동 저장 중단] 현재 씬에서는 자동 저장 안 함: {SceneManager.GetActiveScene().name}");
			ClearAll();
			return;
		}

		Debug.Log("자동 저장 시도");



		foreach (var savable in autoSavables)
		{
			float lastSaved = lastSaveTimes.ContainsKey(savable) ? lastSaveTimes[savable] : 0f;
			float timeSinceLastSave = Time.unscaledTime - lastSaved;

			if (timeSinceLastSave >= saveInterval)
			{
				Debug.Log($"[{savable}] 저장됨 (경과 {timeSinceLastSave:F1}s)");
				savable.AutoSave();
				lastSaveTimes[savable] = Time.unscaledTime;
			}
			else
			{
				Debug.Log($"[{savable}] 저장 건너뜀 (경과 {timeSinceLastSave:F1}s)");
			}
		}
	}

	public void ClearAll()
	{
		lastSaveTimes.Clear();
		autoSavables.Clear();
	}


	private void Start()
	{
		InvokeRepeating(nameof(AutoSaveAll), 10f, 30f);
	}

	public static AutoSaveManager Instance { get; private set; }

	private Dictionary<IAutoSavable, float> lastSaveTimes = new();
	private List<IAutoSavable> autoSavables = new();

	private const float saveInterval = 20f;

}
