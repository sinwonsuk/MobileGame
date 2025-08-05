using BackEnd;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EmailRegisterPopup : MonoBehaviour
{
	private void Start()
	{
		confirmButton.onClick.AddListener(OnClickRegister);
		nextButton.onClick.AddListener(() => Destroy(gameObject));
	}

	void OnClickRegister()
	{
		string email = emailInputField.text;

		if (string.IsNullOrEmpty(email) || !email.Contains("@"))
		{
			PopupManager.Show("유효한 이메일을 입력해주세요.");
			emailInputField.text = "";
			return;
		}

		Backend.BMember.UpdateCustomEmail(email, callback =>
		{
			if (callback.IsSuccess())
			{
				PopupManager.Show("이메일이 성공적으로 등록되었습니다.", ()=>
				{
					onCompleteCallback?.Invoke(email);
					Destroy(this.gameObject);
				});
				
			}
			else
			{
				PopupManager.Show("등록 실패: " + callback.GetMessage());
			}
		});
	}

	public void SetOnCompleteCallback(Action<string> callback)
	{
		this.onCompleteCallback = callback;
	}

	private Action<string> onCompleteCallback;

	[SerializeField] TMP_InputField emailInputField;
	[SerializeField] Button confirmButton;
	[SerializeField] Button nextButton;
}
