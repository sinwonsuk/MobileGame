using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BackEnd;

public class LoginUI : MonoBehaviour
{
	private void Start()
	{
		ShowLogin();
	}

	public void ShowLogin()
	{
		signUpPanel.SetActive(false);
		loginPanel.SetActive(true);
		nicknamePanel.SetActive(false);
	}

	public void ShowSignUp()
	{
		loginPanel.SetActive(false);
		signUpPanel.SetActive(true);
		nicknamePanel.SetActive(false);
	}

	public void ShowNicknamePanel()
	{
		signUpPanel.SetActive(false);
		loginPanel.SetActive(false);
		nicknamePanel.SetActive(true);
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
						ShowNicknamePanel(); 
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
				CheckNickname();
				Instantiate(Test, Vector3.zero, Quaternion.identity); // 테스트용 오브젝트 생성
			},
			onFailure: (error) =>
			{
				Debug.LogError("로그인 실패: " + error);
			});
	}

	void CheckNickname()
	{
		var bro = Backend.BMember.GetUserInfo();

		if(!bro.IsSuccess())
		{
			Debug.LogError("유저 정보 조회 실패: " + bro.GetMessage());
			return;
		}

		var json = bro.GetReturnValuetoJSON();

		try {
			Debug.Log("[전체 JSON 구조]\n" + json.ToJson());
			var row = json["row"];
			string nickname = row["nickname"].ToString();

			if(string.IsNullOrEmpty(nickname) || nickname == "default" || nickname == "null")
			{
				Debug.Log("닉네임이 설정되지 않았습니다. 닉네임 설정 화면으로 이동합니다.");
				ShowNicknamePanel();
			}
			else
			{
				Debug.Log("이미 닉네임이 설정되어 있습니다: " + nickname);
				// 메인 화면으로 이동하거나 게임 시작 로직 추가
			}
		}
		catch (System.Exception e)
		{
			Debug.Log("닉네임 정보가 없습니다. 닉네임 설정 화면으로 이동합니다. \n" + e);
			ShowNicknamePanel();
		}
	}

	public void OnClickConfirmNickname()
	{
		string nickname = nicknameInput.text;

		BackendLogin.Instance.UpdateNickname(nickname,
	    onSuccess: () =>
		{
			Debug.Log("닉네임 설정 성공: " + nickname);
			BackendGameData.userData.nickname = nickname; // 닉네임 업데이트
			BackendGameData.Instance.GameDataUpdate(); // 게임 데이터 업데이트
													   // 메인 화면으로 이동하거나 게임 시작 로직 추가
			Instantiate(Test, Vector3.zero, Quaternion.identity); // 테스트용 오브젝트 생성
		},
		onFailure: (error) =>
		{
			Debug.LogError("닉네임 설정 실패: " + error);
		});

	}

	[SerializeField] GameObject signUpPanel;
	[SerializeField] GameObject loginPanel;
	[SerializeField] GameObject nicknamePanel;

	[SerializeField] private TMP_InputField loginIdInput;
	[SerializeField] private TMP_InputField loginPwInput;

	[SerializeField] private TMP_InputField signUpIdInput;
	[SerializeField] private TMP_InputField SignPwInput;

	[SerializeField] private TMP_InputField nicknameInput;

	[SerializeField] GameObject Test;
}
