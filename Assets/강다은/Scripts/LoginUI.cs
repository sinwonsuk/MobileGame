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
			},
			onFailure: (error) =>
			{
				Debug.LogError("�α��� ����: " + error);
			});
	}
}
