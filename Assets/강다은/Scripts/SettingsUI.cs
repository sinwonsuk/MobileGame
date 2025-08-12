using BackEnd;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static BackEnd.Quobject.SocketIoClientDotNet.Parser.Parser.Encoder;

public class SettingsUI : MonoBehaviour
{
	private void Start()
	{
		Backend.BMember.GetUserInfo(callback =>
		{
			if (callback.IsSuccess())
			{
				var json = callback.GetReturnValuetoJSON();

				var row = json["row"];
				if (row.ContainsKey("emailForFindPassword") && row["emailForFindPassword"] != null)
				{
					emailText.text = row["emailForFindPassword"].ToString();
				}
				else
				{
					emailText.text = "(이메일 미등록)";
				}

			}
		});

		nicknameText.text = BackendGameData.Instance.userData.nickname;

		changeNicknameButton.onClick.AddListener(OpenNicknamePopup);
		confirmNicknameButton.onClick.AddListener(SubmitNicknameChange);
		cancelNicknameButton.onClick.AddListener(CloseNicknamePopup);
		cancelSettingsButton.onClick.AddListener(CloseSettingsPanel);
		logoutButton.onClick.AddListener(OnClickLogout);
		emailRegister.onClick.AddListener(ShowEmailRegisterPopup);

		pwChange.onClick.AddListener(ShowPasswordSettingPanel);
		CancleButton.onClick.AddListener(HidePasswordSettingPanel);
		changePwButton.onClick.AddListener(OnClickChangePassword);

		privacyPolicyButton.onClick.AddListener(() => { SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click, false); Application.OpenURL(privacyPolicyUrl); });
		deleteAccountButton.onClick.AddListener(() => { SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click, false); Application.OpenURL(deleteAccountUrl); });

		bgmSlider.value = PlayerPrefs.GetFloat("BGMVolume", 0.2f);
		sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0.5f);

		ApplyVolumes();

		// 슬라이더 값 변경시 볼륨 업데이트
		bgmSlider.onValueChanged.AddListener(OnBgmChanged);
		sfxSlider.onValueChanged.AddListener(OnSfxChanged);

		nicknamePopup.SetActive(false);
		settingsPanel.SetActive(false);
	}

	private void OnBgmChanged(float volume)
	{
		SoundManager.GetInstance().SetSoundBgm(volume);
		PlayerPrefs.SetFloat("BGMVolume", volume);
	}

	private void OnSfxChanged(float volume)
	{
		SoundManager.GetInstance().sfxVolume = volume;
		SoundManager.GetInstance().UpdateSfxVolumes();
		PlayerPrefs.SetFloat("SFXVolume", volume);
	}

	private void ApplyVolumes()
	{
		SoundManager.GetInstance().SetSoundBgm(bgmSlider.value);
		SoundManager.GetInstance().sfxVolume = sfxSlider.value;
		SoundManager.GetInstance().UpdateSfxVolumes();
	}
	void OpenNicknamePopup()
	{
		SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click, false);
		nicknamePopup.SetActive(true);
		nicknameInput.text = "";
	}

	void CloseNicknamePopup()
	{
		SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click, false);
		nicknamePopup.SetActive(false);
	}

	void CloseSettingsPanel()
	{
		SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click, false);
		settingsPanel.SetActive(false);
	}

	void ShowEmailRegisterPopup()
	{
		SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click, false);
		StartCoroutine(PopupManager.ShowEmailRegisterPopup((email) => { emailText.text = email; }));

	}

	void ShowPasswordSettingPanel()
	{
		SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click, false);
		pwSettingPopup.SetActive(true);
		currentPwInput.text = "";
		newPwInput.text = "";
		confirmPwInput.text = "";
	}

	void HidePasswordSettingPanel()
	{
		SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click, false);
		pwSettingPopup.SetActive(false);
	}

	public void OnClickChangePassword()
	{
		SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click, false);
		string currentPw = currentPwInput.text;
		string newPw = newPwInput.text;
		string confirmPw = confirmPwInput.text;

		if (newPw != confirmPw)
		{
			PopupManager.Show("새 비밀번호가 일치하지 않습니다.");
			return;
		}

		Backend.BMember.ConfirmCustomPassword(currentPw, confirmCallback =>
		{
			if (confirmCallback.IsSuccess())
			{
				Backend.BMember.UpdatePassword(currentPw, newPw, updateCallback =>
				{
					if (updateCallback.IsSuccess())
					{
						PopupManager.Show("비밀번호가 성공적으로 변경되었습니다.", ()=>
						{
							HidePasswordSettingPanel();
						});
						
					}
					else
					{
						string errorCode = confirmCallback.GetErrorCode();
						string message = confirmCallback.GetMessage();
						int.TryParse(confirmCallback.GetStatusCode(), out int statusCode);

						if (statusCode == 400 && errorCode == "BadParameterException" && message.Contains("bad password is not match"))
						{
							PopupManager.Show("기존 비밀번호가 일치하지 않습니다. 다시 확인해 주세요.", () =>
							{
								currentPwInput.text = "";
								newPwInput.text = "";
								confirmPwInput.text = "";
							});
						}

					}
				});
			}
			else
			{
				PopupManager.Show("현재 비밀번호가 올바르지 않습니다.");
			}
		});
	}


	void SubmitNicknameChange()
	{
		SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click, false);
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
					BackendGameData.Instance.userData.nickname = newNickname;

					CloseNicknamePopup();
					nicknameText.text = newNickname;
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

	public void OnClickLogout()
	{
		SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click, false);
		PopupManager.Show("정말 로그아웃 하시겠습니까?", () =>
		{
			Backend.BMember.Logout();
			SoundManager.GetInstance().Bgm_Stop();
			SceneManager.LoadScene("DaniTest");
		});
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

	[Header("Logout")]
	[SerializeField] private Button logoutButton;

	[Header("Texts")]
	[SerializeField] private TextMeshProUGUI emailText;
	[SerializeField] private TextMeshProUGUI nicknameText;

	[Header("Buttons")]
	[SerializeField] private Button emailRegister;
	[SerializeField] private Button pwChange;

	[Header("PWSetting")]
	[SerializeField] private GameObject pwSettingPopup;
	[SerializeField] private TMP_InputField currentPwInput;
	[SerializeField] private TMP_InputField newPwInput;
	[SerializeField] private TMP_InputField confirmPwInput;
	[SerializeField] private Button changePwButton;
	[SerializeField] private Button CancleButton;

	private string deleteAccountUrl = "https://storage.thebackend.io/1ea3f14d34e89530ea88b3245bc82dc17d5f52ce1554049f19fce9219a847cfce18bb88949ceff97e661eeb9a3bb828c69c5513c1e8700aec55b0fa6edd7a5ea14603f7a7268841be4987142de/withdraw/ko/index.html#/customLogin";
	private string privacyPolicyUrl = "https://storage.thebackend.io/1585238bf7ffe74a960bde13f7e1258f4a8836d27df3b152aef67fd7839b1fc6/privacy.html"; 

}
