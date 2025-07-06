using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 뒤끝 SDK namespace 추가
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
        Debug.Log("회원가입을 요청합니다.");

        var bro = Backend.BMember.CustomSignUp(id, pw);

        if (bro.IsSuccess())
        {
            Debug.Log("회원가입에 성공했습니다. : " + bro);
            onSuccess?.Invoke();
		}
        else
        {
            string errorMessage = bro.GetErrorCode() + " - " + bro.GetMessage();
			Debug.LogError("회원가입 실패 : " + errorMessage);
            onFailure?.Invoke(errorMessage);
		}
    }

    public void CustomLogin(string id, string pw, System.Action onSuccess = null, Action<string> onFailure = null)
    {
        Debug.Log("로그인을 요청합니다.");

        var bro = Backend.BMember.CustomLogin(id, pw);

        if (bro.IsSuccess())
        {
            Debug.Log("로그인이 성공했습니다. : " + bro);
            onSuccess?.Invoke();
		}
        else
        {
			string errorMessage = bro.GetErrorCode() + " - " + bro.GetMessage();
			Debug.LogError("로그인 실패 : " + errorMessage);
            onFailure?.Invoke(errorMessage);
		}
    }

    public void UpdateNickname(string nickname, Action onSuccess = null, Action<string> onFailure = null)
    {
        var bro = Backend.BMember.UpdateNickname(nickname);

		if (bro.IsSuccess())
		{
			Debug.Log("닉네임 설정 성공: " + bro);
			onSuccess?.Invoke();
		}
		else
		{
			string errorMessage = bro.GetErrorCode() + " - " + bro.GetMessage();
			Debug.LogError("닉네임 설정 실패 : " + errorMessage);
			onFailure?.Invoke(errorMessage);
		}
	}
}