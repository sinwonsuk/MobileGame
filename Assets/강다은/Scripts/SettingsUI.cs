using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class SettingsUI : MonoBehaviour
{
	private void Start()
	{
		changeNicknameButton.onClick.AddListener(OpenNicknamePopup);
		confirmNicknameButton.onClick.AddListener(SubmitNicknameChange);
		cancelNicknameButton.onClick.AddListener(CloseNicknamePopup);
		cancelSettingsButton.onClick.AddListener(CloseSettingsPanel);

		privacyPolicyButton.onClick.AddListener(() => Application.OpenURL(privacyPolicyUrl));
		deleteAccountButton.onClick.AddListener(() => Application.OpenURL(deleteAccountUrl));

		nicknamePopup.SetActive(false);
		settingsPanel.SetActive(false);
	}

	void OpenNicknamePopup()
	{
		nicknamePopup.SetActive(true);
		nicknameInput.text = "";
	}

	void CloseNicknamePopup()
	{
		nicknamePopup.SetActive(false);
	}

	void CloseSettingsPanel()
	{
		settingsPanel.SetActive(false);
	}

	void SubmitNicknameChange()
	{
		string newNickname = nicknameInput.text.Trim();

		if (string.IsNullOrEmpty(newNickname))
		{
			PopupManager.Show("닉네임을 입력하세요.", () => {
				nicknameInput.text = ""; // 초기화
			});
			return;
		}

		if (newNickname.Length > 20)
		{
			PopupManager.Show("닉네임은 20자 이하로 입력해주세요.", () => {
				nicknameInput.text = "";
			});
			return;
		}

		BackendLogin.Instance.UpdateNickname(newNickname,
			onSuccess: () =>
			{
				PopupManager.Show("닉네임 변경 완료!", () => {
					CloseNicknamePopup();
				});
			},
			onFailure: (error) =>
			{
				if (error.Contains("DuplicatedParameterException"))
				{
					PopupManager.Show("이미 사용 중인 닉네임입니다.", () => {
						nicknameInput.text = "";
					});
				}
				else if (error.Contains("bad beginning or end of"))
				{
					PopupManager.Show("닉네임 앞뒤 공백은 제거해주세요.", () => {
						nicknameInput.text = "";
					});
				}
				else
				{
					PopupManager.Show("알 수 없는 오류가 발생했습니다.", () => {
						nicknameInput.text = "";
					});
				}
			}
		);
	}


	[Header("Setting Panel")]
	[SerializeField] private GameObject settingsPanel;
	[SerializeField] private Button cancelSettingsButton;

	[Header("Volume Sliders")]
	[SerializeField] private Slider sfxSlider;
	[SerializeField] private Slider bgmSlider;

	[Header("Nickname UI")]
	[SerializeField] private GameObject nicknamePopup;
	[SerializeField] private TMP_InputField nicknameInput;
	[SerializeField] private Button changeNicknameButton;
	[SerializeField] private Button confirmNicknameButton;
	[SerializeField] private Button cancelNicknameButton;

	[Header("External Links")]
	[SerializeField] private Button privacyPolicyButton;
	[SerializeField] private Button deleteAccountButton;

	private string deleteAccountUrl = "https://storage.thebackend.io/1ea3f14d34e89530ea88b3245bc82dc17d5f52ce1554049f19fce9219a847cfce18bb88949ceff97e661eeb9a3bb828c69c5513c1e8700aec55b0fa6edd7a5ea14603f7a7268841be4987142de/withdraw/ko/index.html#/customLogin";
	private string privacyPolicyUrl = "https://storage.thebackend.io/1585238bf7ffe74a960bde13f7e1258f4a8836d27df3b152aef67fd7839b1fc6/privacy.html"; 

}
