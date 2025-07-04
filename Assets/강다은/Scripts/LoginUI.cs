using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoginUI : MonoBehaviour
{
	[SerializeField] GameObject signUpPanel; 
	[SerializeField] GameObject loginPanel; 

	[SerializeField] private TMP_InputField loginIdInput;
	[SerializeField] private TMP_InputField loginPwInput;

	[SerializeField] private TMP_InputField signUpIdInput;
	[SerializeField] private TMP_InputField SignPwInput;

	private void Start()
	{
		ShowLogin();
	}

	public void ShowLogin()
	{
		signUpPanel.SetActive(false);
		loginPanel.SetActive(true);
	}

	public void ShowSignUp()
	{
		loginPanel.SetActive(false);
		signUpPanel.SetActive(true);
	}


	// 회원가입
	public void OnClickSignUp()
	{
		string id = signUpIdInput.text;
		string pw = SignPwInput.text;

		if (string.IsNullOrEmpty(id))
		{
			Debug.LogError("아이디를 입력해주세요.");
			return;
		}
		if (string.IsNullOrEmpty(pw))
		{
			Debug.LogError("비밀번호를 입력해주세요.");
			return;
		}

		BackendLogin.Instance.CustomSignUp(id, pw,
			onSuccess: () =>
			{
				Debug.Log("회원가입 성공");

				BackendLogin.Instance.CustomLogin(id, pw,
					onSuccess: () =>
					{
						Debug.Log("로그인 성공 후 데이터 불러오기 시도");
						BackendGameData.Instance.GameDataGetOrInsert(); // 로그인 성공 후 데이터 불러오기
					},
					onFailure: (error) =>
					{
						Debug.LogError("로그인 실패: " + error);
					});
			},
			onFailure: (error) =>
			{
				Debug.LogError("회원가입 실패: " + error);
			});
	}

	// 로그인
	public void OnClickLogin()
	{
		string id = loginIdInput.text;
		string pw = loginPwInput.text;

		if (string.IsNullOrEmpty(id))
		{
			Debug.LogError("아이디를 입력해주세요.");
			return;
		}
		if (string.IsNullOrEmpty(pw))
		{
			Debug.LogError("비밀번호를 입력해주세요.");
			return;
		}

		BackendLogin.Instance.CustomLogin(id, pw,
			onSuccess: () =>
			{
				Debug.Log("로그인 성공");
				BackendGameData.Instance.GameDataGetOrInsert(); // 로그인 성공 후 데이터 불러오기
			},
			onFailure: (error) =>
			{
				Debug.LogError("로그인 실패: " + error);
			});
	}
}
