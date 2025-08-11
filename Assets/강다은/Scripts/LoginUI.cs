using BackEnd;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour
{
    private void OnDisable()
    {
        EventBus<ChangeLoadImageEvent>.OnEvent -= LoadSencmImage;
    }
    private void OnEnable()
    {
        EventBus<ChangeLoadImageEvent>.OnEvent += LoadSencmImage;
    }



    private IEnumerator Start()
	{
		mainImage.gameObject.SetActive(true);
		loadingImage.gameObject.SetActive(false);

		// Backend 초기화될 때까지 기다림
		while (!Backend.IsInitialized)
		{
			yield return null;
		}

		Debug.Log("Backend 초기화 완료됨, 자동 로그인 시도");

		// 자동 로그인 시도
		Backend.BMember.LoginWithTheBackendToken(callback =>
		{
			if (callback.IsSuccess())
			{
				Debug.Log("BackendToken 자동 로그인 성공");
				StartCoroutine(LoginFlowCoroutine());
			}
			else
			{
				Debug.Log("자동 로그인 실패, 수동 로그인 화면으로");
				mainImage.gameObject.SetActive(false);
				ShowLogin();
			}
		});
	}

	public void ShowLogin()
	{
		SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click, false);
		signUpPanel.SetActive(false);
		loginPanel.SetActive(true);
		nicknamePanel.SetActive(false);
	}

	public void ShowSignUp()
	{
		SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click, false);
		loginPanel.SetActive(false);
		signUpPanel.SetActive(true);
		nicknamePanel.SetActive(false);
	}

	public void ShowNicknamePanel()
	{
		SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click, false);
		signUpPanel.SetActive(false);
		loginPanel.SetActive(false);
		nicknamePanel.SetActive(true);
	}


	// 회원가입
	public void OnClickSignUp()
	{
		SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click, false);

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
		SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click, false);

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

	public void OnClickGoogleLoginButton()
	{
		SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click, false);
		StartGoogleLogin();
	}

	private void StartGoogleLogin()
	{
		TheBackend.ToolKit.GoogleLogin.Android.GoogleLogin(OnGoogleLoginCallback);
	}

	private void OnGoogleLoginCallback(bool isSuccess, string errorMessage, string token)
	{
		if (!isSuccess)
		{
			Debug.LogError("구글 로그인 실패: " + errorMessage);
			PopupManager.Show("[구글 로그인 실패]\n" + "해당 로그인은 Android에서만 지원됩니다.");
			return;
		}

		Debug.Log("구글 토큰: " + token);

		var bro = Backend.BMember.AuthorizeFederation(token, FederationType.Google);
		if (bro.IsSuccess())
		{
			Debug.Log("페더레이션 로그인 성공!");
			StartCoroutine(LoginFlowCoroutine());
		}
		else
		{
			Debug.LogError("서버 로그인 실패: " + bro.GetMessage());
			PopupManager.Show("서버 로그인 실패\n" + bro.GetMessage());
		}
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

		string email = null;
		if (json["row"].ContainsKey("emailForFindPassword"))
		{
			email = json["row"]["emailForFindPassword"]?.ToString();
		}

		if (string.IsNullOrEmpty(email) || email == "null")
		{
			Debug.Log("[이메일 미등록] 이메일 등록 팝업 호출");
			yield return PopupManager.ShowEmailRegisterPopup((email) => Debug.Log(email));
		}
		else
		{
			Debug.Log("[이메일 등록됨]");
		}

		//닉네임 체크
		string nickname = json["row"]["nickname"]?.ToString();
		bool needsNickname = string.IsNullOrEmpty(nickname) || nickname == "default" || nickname == "null";

		if (needsNickname)
		{
			PopupManager.Show("닉네임이 설정되지 않았습니다.\n닉네임 설정 화면으로 이동합니다.", () =>
			{
				ShowNicknamePanel();
			});
		}
		else
		{
			Debug.Log("이미 닉네임이 설정되어 있습니다: " + nickname);

            SceneChange.Instance.LoadSceneAsync(SceneName.SampleScene);

            //StartCoroutine(LoadSceneAsync("SampleScene"));
		}
	}


	public void OnClickConfirmNickname()
	{
		SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click, false);

		string nickname = nicknameInput.text.Trim();

		if (string.IsNullOrEmpty(nickname))
		{
			PopupManager.Show("닉네임을 입력하세요.", () => {
				nicknameInput.text = ""; // 초기화
			});
			return;
		}

		if (nickname.Length > 20)
		{
			PopupManager.Show("닉네임은 20자 이하로 입력해주세요.", () => {
				nicknameInput.text = "";
			});
			return;
		}

		BackendLogin.Instance.UpdateNickname(nickname,
			onSuccess: () =>
			{
				PopupManager.Show("닉네임 변경 완료!", () => {

					BackendGameData.Instance.userData.nickname = nickname; // 닉네임 업데이트
					BackendGameData.Instance.GameDataUpdate(); // 게임 데이터 업데이트

                    SceneChange.Instance.LoadSceneAsync(SceneName.SampleScene);
                    //StartCoroutine(LoadSceneAsync("SampleScene"));
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

		// 유저 인벤토리 존재 확인 및 데이터 삽입
		string ownerIndate = Backend.UserInDate;
		yield return StartCoroutine(InventoryManager.Instance.InsertInventoryIfNotExists(ownerIndate));
		yield return StartCoroutine(InventoryManager.Instance.LoadUserInventory(ownerIndate));

		// 직원 데이터 삽입
		yield return StartCoroutine(EmployeeManager.Instance.InsertEmployeesIfNotExists(ownerIndate));
		yield return StartCoroutine(EmployeeManager.Instance.LoadEmployeeData(ownerIndate));

		bool isDone = false;
		BackendGameData.Instance.GameDataGetOrInsert(() =>
		{
			isDone = true;
		});
		yield return new WaitUntil(() => isDone);

		if (IsAdminAccount())
		{
			Debug.Log("<관리자> StaticData + CSV 시작");

			// StaticData 초기화
			GameObject initializerGO = Instantiate(staticDataInitializer, Vector3.zero, Quaternion.identity);
			var initializer = initializerGO.GetComponent<StaticDataInitializer>();

			if (initializer == null)
			{
				Debug.LogError("StaticDataInitializer 없음");
				yield break;
			}

			yield return StartCoroutine(initializer.InitializeAllStaticData());
			Debug.Log("정적 데이터 초기화 완료");

			// CSV 업로더
			GameObject uploaderGO = Instantiate(csvUploader, Vector3.zero, Quaternion.identity);
			var uploader = uploaderGO.GetComponent<CSVTableUploader>();

			if (uploader == null)
			{
				Debug.LogError("CSVUploader 없음");
				yield break;
			}

			bool uploadDone = false;
			uploader.onComplete = () =>
			{
				uploadDone = true;
			};

			yield return new WaitUntil(() => uploadDone);
			Debug.Log("CSV 업로드 완료, 씬 이동");


            SceneChange.Instance.LoadSceneAsync(SceneName.SampleScene);

            //StartCoroutine(LoadSceneAsync("SampleScene"));

            //SceneManager.LoadScene("SampleScene");
		}
		else
		{
			// 일반 유저 -> 닉네임 검사 후 씬 이동
			yield return StartCoroutine(CheckNicknameAndProceed());
		}

        //
    }


    public IEnumerator LoadSceneAsync(string sceneName)
    {
		
		//AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        //while (!op.isDone)
        //{
        //    yield return null;
        //}
        yield return null;
        //loadingImage.gameObject.SetActive(false);
    }

	public void LoadSencmImage(ChangeLoadImageEvent changeLoadImageEvent)
	{
		bool isLoading = changeLoadImageEvent.isLoading;


        Sprite loadedSprite = Resources.Load<Sprite>("MainImage");
        if (loadedSprite == null)
        {
            Debug.LogError("MainImage 스프라이트를 찾을 수 없습니다.");
            return;
        }

        mainImage.sprite = loadedSprite;


        mainImage.gameObject.SetActive(isLoading);
    }




    [SerializeField] private GameObject signUpPanel;
	[SerializeField] private GameObject loginPanel;
	[SerializeField] private GameObject nicknamePanel;

	[SerializeField] private TMP_InputField loginIdInput;
	[SerializeField] private TMP_InputField loginPwInput;

	[SerializeField] private TMP_InputField signUpIdInput;
	[SerializeField] private TMP_InputField SignPwInput;

	[SerializeField] private TMP_InputField nicknameInput;

	[SerializeField] private GameObject csvUploader;

	[SerializeField] private GameObject staticDataInitializer;

    [SerializeField] private Image loadingImage;
	[SerializeField] private Image mainImage;

    [SerializeField] private Transform mainImageTransform;
}
