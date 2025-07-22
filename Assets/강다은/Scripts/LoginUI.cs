using BackEnd;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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
			PopupManager.Show("Please Enter the ID.");
			return;
		}
		if (string.IsNullOrEmpty(pw))
		{
			PopupManager.Show("Please Enter the Password.");
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
						PopupManager.Show("로그인에 실패했습니다.\n" + error);
					});
                
            },
			onFailure: (error) =>
			{
				PopupManager.Show("회원가입에 실패했습니다.\n" + error);
			});
	}

	// 로그인
	public void OnClickLogin()
	{
		string id = loginIdInput.text;
		string pw = loginPwInput.text;

		if (string.IsNullOrEmpty(id))
		{
			PopupManager.Show("Please Enter the ID.");
			return;
		}
		if (string.IsNullOrEmpty(pw))
		{
			PopupManager.Show("Please Enter the Password.");
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
				PopupManager.Show("로그인에 실패했습니다.\n" + error);
			});

    }

	private IEnumerator CheckNicknameAndProceed()
	{
		var bro = Backend.BMember.GetUserInfo();
		if (!bro.IsSuccess())
		{
			Debug.LogError("유저 정보 조회 실패: " + bro.GetMessage());
			PopupManager.Show("정보 조회에 실패했습니다.\n");
			yield break;
		}

		var json = bro.GetReturnValuetoJSON();
		try
		{
			Debug.Log("[전체 JSON 구조]\n" + json.ToJson());

			if (!json.ContainsKey("row") || json["row"] == null)
			{
				Debug.LogWarning("'row' 키 없음 또는 null");
				PopupManager.Show("유저 정보가 올바르지 않습니다. 닉네임 설정으로 이동합니다.", () =>
				{
					ShowNicknamePanel();
				});
				yield break;
			}

			var row = json["row"];

			if (!row.ContainsKey("nickname") || row["nickname"] == null)
			{
				Debug.LogWarning("'nickname' 키 없음 또는 null");
				PopupManager.Show("닉네임이 없습니다. 설정 화면으로 이동합니다.", () =>
				{
					ShowNicknamePanel();
				});
				yield break;
			}

			string nickname = row["nickname"].ToString();

			if (string.IsNullOrEmpty(nickname) || nickname == "default" || nickname == "null" || string.IsNullOrEmpty(row["nickname"].ToString()))
			{
				PopupManager.Show("닉네임이 설정되지 않았습니다.\n닉네임 설정 화면으로 이동합니다.", () =>
				{
					ShowNicknamePanel();
				});
			}
			else
			{
				Debug.Log("이미 닉네임이 설정되어 있습니다: " + nickname);
				SceneManager.LoadScene("SampleScene");
			}
		}
		catch (System.Exception e)
		{
			Debug.LogError("닉네임 정보 조회 중 오류 발생: " + e.Message);
			PopupManager.Show("닉네임 확인 중 오류가 발생했습니다.\n설정 화면으로 이동합니다.", () =>
			{
				ShowNicknamePanel();
			});
		}
	}


	public void OnClickConfirmNickname()
	{
		string nickname = nicknameInput.text;

		BackendLogin.Instance.UpdateNickname(nickname,
	    onSuccess: () =>
		{
			Debug.Log("닉네임 설정 성공: " + nickname);
			BackendGameData.Instance.userData.nickname = nickname; // 닉네임 업데이트
			BackendGameData.Instance.GameDataUpdate(); // 게임 데이터 업데이트

			SceneManager.LoadScene("SampleScene");
		},
		onFailure: (error) =>
		{
			PopupManager.Show("닉네임 설정에 실패하였습니다.");
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
		BackendGameData.Instance.GameDataGetOrInsert(() => 
		{
			if (IsAdminAccount())
			{
				Debug.Log("<관리자> 계정입니다. StaticData 삽입");

				GameObject uploader = Instantiate(csvUploader, Vector3.zero, Quaternion.identity);
				uploader.GetComponent<CSVTableUploader>().onComplete = () =>
				{
					Debug.Log("<관리자> CSV 업로드 완료. 씬 이동 시작");
					SceneManager.LoadScene("SampleScene");
				};
			}
			else
			{
				// 일반 유저 → 닉네임 검사 & 씬 이동
				StartCoroutine(CheckNicknameAndProceed());
			}
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

	[SerializeField] GameObject csvUploader;

	[SerializeField] private StaticDataInitializer staticDataInitializer;
}
