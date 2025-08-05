using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections;

public class PopupManager : MonoBehaviour
{
	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}

		popupPanel.SetActive(false);

		confirmButton.onClick.AddListener(() =>
		{
			popupPanel.SetActive(false);
			onConfirmCallback?.Invoke();
			onConfirmCallback = null;
		});
	}


	// 확인 버튼 있는데 콜백 없음
	public static void Show(string message)
	{
		Show(message, null);
	}

	// 확인 버튼 있는 팝업 콜백 있음
	public static void Show(string message, Action onConfirm)
	{
		if (Instance == null)
		{
			Debug.LogWarning("PopupManager 인스턴스가 없음");
			return;
		}

		Instance.popupPanel.SetActive(true);
		Instance.messageText.text = message;
		Instance.onConfirmCallback = onConfirm;
	}

	public static IEnumerator ShowEmailRegisterPopup(Action<string> onComplete)
	{
		bool done = false;
		string enteredEmail = null;

		EmailRegisterPopup popup = Instantiate(Instance.emailPopupPrefab);
		popup.SetOnCompleteCallback((email) =>
		{
			enteredEmail = email;
			done = true;
		});

		yield return new WaitUntil(() => done);

		onComplete?.Invoke(enteredEmail);
	}



	public static PopupManager Instance;

	[SerializeField] private GameObject popupPanel;
	[SerializeField] private TMP_Text messageText;
	[SerializeField] private Button confirmButton;
	[SerializeField] private EmailRegisterPopup emailPopupPrefab;

	private Action onConfirmCallback;
}
