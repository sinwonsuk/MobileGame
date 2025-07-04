using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �ڳ� SDK namespace �߰�
using BackEnd;
using System;

public class BackendLogin
{
    private static BackendLogin _instance = null;

    public static BackendLogin Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new BackendLogin();
            }

            return _instance;
        }
    }

    public void CustomSignUp(string id, string pw, System.Action onSuccess = null, Action<string> onFailure = null)
    {
        Debug.Log("ȸ�������� ��û�մϴ�.");

        var bro = Backend.BMember.CustomSignUp(id, pw);

        if (bro.IsSuccess())
        {
            Debug.Log("ȸ�����Կ� �����߽��ϴ�. : " + bro);
            onSuccess?.Invoke();
		}
        else
        {
            string errorMessage = bro.GetErrorCode() + " - " + bro.GetMessage();
			Debug.LogError("ȸ������ ���� : " + errorMessage);
            onFailure?.Invoke(errorMessage);
		}
    }

    public void CustomLogin(string id, string pw, System.Action onSuccess = null, Action<string> onFailure = null)
    {
        Debug.Log("�α����� ��û�մϴ�.");

        var bro = Backend.BMember.CustomLogin(id, pw);

        if (bro.IsSuccess())
        {
            Debug.Log("�α����� �����߽��ϴ�. : " + bro);
            onSuccess?.Invoke();
		}
        else
        {
			string errorMessage = bro.GetErrorCode() + " - " + bro.GetMessage();
			Debug.LogError("�α��� ���� : " + errorMessage);
            onFailure?.Invoke(errorMessage);
		}
    }

    public void UpdateNickname(string nickname, Action onSuccess = null, Action<string> onFailure = null)
    {
        var bro = Backend.BMember.UpdateNickname(nickname);

		if (bro.IsSuccess())
		{
			Debug.Log("�г��� ���� ����: " + bro);
			onSuccess?.Invoke();
		}
		else
		{
			string errorMessage = bro.GetErrorCode() + " - " + bro.GetMessage();
			Debug.LogError("�г��� ���� ���� : " + errorMessage);
			onFailure?.Invoke(errorMessage);
		}
	}
}