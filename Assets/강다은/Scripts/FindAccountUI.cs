using BackEnd;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;

public class FindAccountUI : MonoBehaviour
{
	private void Start()
	{
		btnOpenPanel.onClick.AddListener(OnClickOpenPanel);
		btnClosePanel.onClick.AddListener(OnClickClosePanel);
		btnFindID.onClick.AddListener(OnClickFindID);
		btnResetPW.onClick.AddListener(OnClickResetPassword);

		toggleFindID.onValueChanged.AddListener(_ => OnToggleChanged());
		toggleResetPW.onValueChanged.AddListener(_ => OnToggleChanged());

		findAccountPanel.SetActive(false);
	}

	private void OnToggleChanged()
	{
		panelFindID.SetActive(toggleFindID.isOn);
		panelResetPW.SetActive(toggleResetPW.isOn);
	}

	public void OnClickOpenPanel()
	{
		findAccountPanel.SetActive(true);

		toggleFindID.isOn = true;
		toggleResetPW.isOn = false;

		OnToggleChanged(); // 수동 동기화
	}

	public void OnClickClosePanel()
	{
		findAccountPanel.SetActive(false);
		panelFindID.SetActive(false);
		panelResetPW.SetActive(false);
		emailInputForID.text = "";
		idInputForPW.text = "";
		emailInputForPW.text = "";
	}

	public void OnClickFindID()
	{
		string email = emailInputForID.text;
		if (string.IsNullOrEmpty(email) || !email.Contains("@"))
		{
			PopupManager.Show("유효한 이메일을 입력해주세요.");
			return;
		}

		Backend.BMember.FindCustomID(email, callback =>
		{
			if (callback.IsSuccess())
				PopupManager.Show("이메일로 아이디 정보를 전송했습니다.", () => { findAccountPanel.SetActive(false); });
			else
				HandleFindIDError(callback, emailInputForID);
		});
	}

	public void OnClickResetPassword()
	{
		string id = idInputForPW.text;
		string email = emailInputForPW.text;

		if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(email))
		{
			PopupManager.Show("아이디와 이메일을 모두 입력해주세요.");
			return;
		}

		Backend.BMember.ResetPassword(id, email, callback =>
		{
			if (callback.IsSuccess())
				PopupManager.Show("이메일로 초기화된 비밀번호를 전송했습니다.", () => { findAccountPanel.SetActive(false); });
			else
				HandleResetPasswordError(callback, idInputForPW, emailInputForPW);
		});
	}

	void HandleFindIDError(BackendReturnObject callback, TMP_InputField emailInput)
	{
		string message = callback.GetMessage();
		string errorCode = callback.GetErrorCode();
		int.TryParse(callback.GetStatusCode(), out int statusCode);

		switch (statusCode)
		{
			case 204:
				PopupManager.Show("이메일이 전송되었습니다. 메일함을 확인해 주세요.");
				break;

			case 400:
				if (errorCode == "InvalidParameterValue" && message.Contains("no-reply@backnd.com"))
				{
					PopupManager.Show("프로젝트 이름에 특수문자가 포함되어 있어 이메일을 보낼 수 없습니다.\n콘솔 설정을 확인해 주세요.");
				}
				else if (errorCode == "BadParameterException" && message.Contains("bad email is not match"))
				{
					PopupManager.Show("입력한 이메일이 잘못되었거나 등록된 이메일이 아닙니다.", () =>
					{
						emailInput.text = "";
					});
				}
				else
				{
					PopupManager.Show("요청에 문제가 발생했습니다: " + message);
				}
				break;

			case 404:
				if (message.Contains("gamer not found"))
				{
					PopupManager.Show("입력한 이메일에 해당하는 계정이 없습니다.", () =>
					{
						emailInput.text = "";
					});
				}
				else
				{
					PopupManager.Show("요청한 정보를 찾을 수 없습니다.");
				}
				break;

			case 429:
				PopupManager.Show("요청 횟수를 초과했습니다.\n하루 최대 5회까지 요청 가능합니다. 내일 다시 시도해 주세요.");
				break;

			default:
				PopupManager.Show($"알 수 없는 오류가 발생했습니다.\n({statusCode}) {message}");
				break;
		}
	}


	void HandleResetPasswordError(BackendReturnObject callback, TMP_InputField idInput, TMP_InputField emailInput)
	{
		string message = callback.GetMessage();
		string errorCode = callback.GetErrorCode();
		int.TryParse(callback.GetStatusCode(), out int statusCode);

		switch (statusCode)
		{
			case 204:
				PopupManager.Show("이메일이 전송되었습니다. 메일함을 확인해 주세요.");
				break;

			case 400:
				if (errorCode == "InvalidParameterValue" && message.Contains("no-reply@backnd.com"))
				{
					PopupManager.Show("프로젝트 이름에 특수문자가 포함되어 있어 이메일을 보낼 수 없습니다.\n콘솔 설정을 확인해 주세요.");
				}
				else if (errorCode == "BadParameterException" && message.Contains("bad email is not match"))
				{
					PopupManager.Show("입력한 이메일이 잘못되었거나 등록된 이메일이 아닙니다.", () =>
					{
						emailInput.text = "";
					});
				}
				else
				{
					PopupManager.Show("요청에 문제가 발생했습니다: " + message);
				}
				break;

			case 404:
				if (message.Contains("enrolled email not found"))
				{
					PopupManager.Show("해당 계정에는 등록된 이메일이 없습니다.", () =>
					{
						emailInput.text = "";
					});
				}
				else if (message.Contains("gamer not found"))
				{
					PopupManager.Show("입력한 ID가 존재하지 않습니다.", () =>
					{
						idInput.text = "";
					});
				}
				else
				{
					PopupManager.Show("요청한 정보를 찾을 수 없습니다.");
				}
				break;

			case 429:
				PopupManager.Show("요청 횟수를 초과했습니다.\n하루 최대 5회까지 요청 가능합니다. 내일 다시 시도해 주세요.");
				break;

			default:
				PopupManager.Show($"알 수 없는 오류가 발생했습니다.\n({statusCode}) {message}");
				break;
		}
	}

	[Header("FindAccountPanel")]
	[SerializeField] GameObject findAccountPanel;
	
	[Header("Buttons")]
	[SerializeField] Button btnOpenPanel;
	[SerializeField] Button btnClosePanel;
	[SerializeField] Button btnFindID;
	[SerializeField] Button btnResetPW;

	[Header("Toggles")]
	[SerializeField] Toggle toggleFindID;
	[SerializeField] Toggle toggleResetPW;

	[Header("Panels")]
	[SerializeField] GameObject panelFindID;
	[SerializeField] GameObject panelResetPW;

	[Header("Find ID")]
	[SerializeField] TMP_InputField emailInputForID;

	[Header("Reset PW")]
	[SerializeField] TMP_InputField idInputForPW;
	[SerializeField] TMP_InputField emailInputForPW;

}
