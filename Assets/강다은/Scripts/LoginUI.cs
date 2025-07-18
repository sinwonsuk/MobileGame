using BackEnd;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
						Debug.Log("로그인 성공, 대기 후 데이터 로드 시작");
						StartCoroutine(LoginFlowCoroutine());
					},
					onFailure: (error) =>
					{
						Debug.LogError("로그인 실패: " + error);
					});
                SceneManager.LoadScene("SampleScene");
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
				StartCoroutine(LoginFlowCoroutine());
			},
			onFailure: (error) =>
			{
				Debug.LogError("로그인 실패: " + error);
			});

		SceneManager.LoadScene("SampleScene");

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
		},
		onFailure: (error) =>
		{
			Debug.LogError("닉네임 설정 실패: " + error);
		});

	}

	// 관리자 계정인지 확인하는 함수
	bool IsAdminAccount()
	{
		var bro = Backend.BMember.GetUserInfo();
		if (!bro.IsSuccess()) return false;

		try
		{
			var json = bro.GetReturnValuetoJSON();
			if (json.ContainsKey("row") && json["row"].ContainsKey("nickname"))
			{
				string nickname = json["row"]["nickname"].ToString();
				return nickname == "no"; // 관리자 닉네임
			}
		}
		catch
		{
			Debug.LogWarning("[Admin Check] 닉네임 정보 없음");
		}

		return false;
	}

	private IEnumerator LoginFlowCoroutine()
	{
		while (string.IsNullOrEmpty(Backend.UserInDate))
		{
			yield return null; // 한 프레임 기다림
		}

		// 정적 테이블 초기화 (server -> scriptable obj)
		yield return StartCoroutine(staticDataInitializer.InitializeAllStaticData());
		Debug.Log("정적 테이블 초기화 완료");

		// 유저 인벤토리 존재 확인 및 데이터 삽입
		string ownerIndate = Backend.UserInDate;
		yield return StartCoroutine(InventoryManager.Instance.InsertInventoryIfNotExists(ownerIndate));

		//인벤토리 데이터 불러오기
		yield return StartCoroutine(InventoryManager.Instance.LoadUserInventory(ownerIndate));

		// 기타 유저 게임 데이터 불러오기
		BackendGameData.Instance.GameDataGetOrInsert();

		// 닉네임 여부 확인
		CheckNickname();

		// 관리자 계정일 경우 Csv -> Server
		if (IsAdminAccount())
		{
			Debug.Log("<관리자> 계정입니다. StaticData 삽입");
			Instantiate(csvUploader, Vector3.zero, Quaternion.identity);
		}
	}

	[SerializeField] GameObject signUpPanel;
	[SerializeField] GameObject loginPanel;
	[SerializeField] GameObject nicknamePanel;

	[SerializeField] private TMP_InputField loginIdInput;
	[SerializeField] private TMP_InputField loginPwInput;

	[SerializeField] private TMP_InputField signUpIdInput;
	[SerializeField] private TMP_InputField SignPwInput;

	[SerializeField] private TMP_InputField nicknameInput;

	[SerializeField] GameObject csvUploader;

	[SerializeField] private StaticDataInitializer staticDataInitializer;
}
