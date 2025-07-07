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


	// ȸ������
	public void OnClickSignUp()
	{
		string id = signUpIdInput.text;
		string pw = SignPwInput.text;

		if (string.IsNullOrEmpty(id))
		{
			Debug.LogError("���̵� �Է����ּ���.");
			return;
		}
		if (string.IsNullOrEmpty(pw))
		{
			Debug.LogError("��й�ȣ�� �Է����ּ���.");
			return;
		}

		BackendLogin.Instance.CustomSignUp(id, pw,
			onSuccess: () =>
			{
				Debug.Log("ȸ������ ����");

				BackendLogin.Instance.CustomLogin(id, pw,
					onSuccess: () =>
					{
						Debug.Log("�α��� ���� �� ������ �ҷ����� �õ�");
						BackendGameData.Instance.GameDataGetOrInsert(); // �α��� ���� �� ������ �ҷ�����
						ShowNicknamePanel(); 
					},
					onFailure: (error) =>
					{
						Debug.LogError("�α��� ����: " + error);
					});
			},
			onFailure: (error) =>
			{
				Debug.LogError("ȸ������ ����: " + error);
			});
	}

	// �α���
	public void OnClickLogin()
	{
		string id = loginIdInput.text;
		string pw = loginPwInput.text;

		if (string.IsNullOrEmpty(id))
		{
			Debug.LogError("���̵� �Է����ּ���.");
			return;
		}
		if (string.IsNullOrEmpty(pw))
		{
			Debug.LogError("��й�ȣ�� �Է����ּ���.");
			return;
		}

		BackendLogin.Instance.CustomLogin(id, pw,
			onSuccess: () =>
			{
				Debug.Log("�α��� ����");
				BackendGameData.Instance.GameDataGetOrInsert(); // �α��� ���� �� ������ �ҷ�����
				CheckNickname();
				Instantiate(Test, Vector3.zero, Quaternion.identity); // �׽�Ʈ�� ������Ʈ ����
			},
			onFailure: (error) =>
			{
				Debug.LogError("�α��� ����: " + error);
			});
	}

	void CheckNickname()
	{
		var bro = Backend.BMember.GetUserInfo();

		if(!bro.IsSuccess())
		{
			Debug.LogError("���� ���� ��ȸ ����: " + bro.GetMessage());
			return;
		}

		var json = bro.GetReturnValuetoJSON();

		try {
			Debug.Log("[��ü JSON ����]\n" + json.ToJson());
			var row = json["row"];
			string nickname = row["nickname"].ToString();

			if(string.IsNullOrEmpty(nickname) || nickname == "default" || nickname == "null")
			{
				Debug.Log("�г����� �������� �ʾҽ��ϴ�. �г��� ���� ȭ������ �̵��մϴ�.");
				ShowNicknamePanel();
			}
			else
			{
				Debug.Log("�̹� �г����� �����Ǿ� �ֽ��ϴ�: " + nickname);
				// ���� ȭ������ �̵��ϰų� ���� ���� ���� �߰�
			}
		}
		catch (System.Exception e)
		{
			Debug.Log("�г��� ������ �����ϴ�. �г��� ���� ȭ������ �̵��մϴ�. \n" + e);
			ShowNicknamePanel();
		}
	}

	public void OnClickConfirmNickname()
	{
		string nickname = nicknameInput.text;

		BackendLogin.Instance.UpdateNickname(nickname,
	    onSuccess: () =>
		{
			Debug.Log("�г��� ���� ����: " + nickname);
			BackendGameData.userData.nickname = nickname; // �г��� ������Ʈ
			BackendGameData.Instance.GameDataUpdate(); // ���� ������ ������Ʈ
													   // ���� ȭ������ �̵��ϰų� ���� ���� ���� �߰�
			Instantiate(Test, Vector3.zero, Quaternion.identity); // �׽�Ʈ�� ������Ʈ ����
		},
		onFailure: (error) =>
		{
			Debug.LogError("�г��� ���� ����: " + error);
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
