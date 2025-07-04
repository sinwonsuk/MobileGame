using UnityEngine;

// �ڳ� SDK namespace �߰�
using BackEnd;

public class BackendManager : MonoBehaviour
{
    void Start()
    {
        var bro = Backend.Initialize(); // �ڳ� �ʱ�ȭ

        // �ڳ� �ʱ�ȭ�� ���� ���䰪
        if (bro.IsSuccess())
        {
            Debug.Log("�ʱ�ȭ ���� : " + bro); // ������ ��� statusCode 204 Success
        }
        else
        {
            Debug.LogError("�ʱ�ȭ ���� : " + bro); // ������ ��� statusCode 400�� ���� �߻�
        }

        UserLogin("user1", "1234"); // [�߰�] �ڳ� �α��� �Լ� ȣ��
	}

    void Test()
    {
        //BackendLogin.Instance.CustomSignUp("user1", "1234"); // [�߰�] �ڳ� ȸ������ �Լ�
        BackendLogin.Instance.CustomLogin("user1", "1234"); // [�߰�] �ڳ� �α���
        //BackendLogin.Instance.UpdateNickname("���ϴ� �̸�"); // [�߰�] �г��� ����

        //BackendGameData.Instance.GameDataInsert();

        BackendGameData.Instance.GameDataGet(); //[�߰�] ������ �ҷ����� �Լ�
        Debug.Log("�׽�Ʈ�� �����մϴ�.");
    }

    void UserLogin(string id, string pw)
	{
		BackendLogin.Instance.CustomLogin(id, pw, () =>
		{
            BackendLogin.Instance.CustomLogin(id, pw, () =>
            {
                Debug.Log("�α��� ���� �� ������ �ҷ����� �õ�");
                BackendGameData.Instance.GameDataGet(); // �α��� ���� �� ������ �ҷ�����
            });

		});
	}
}