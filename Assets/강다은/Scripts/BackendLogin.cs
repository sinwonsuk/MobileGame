using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 뒤끝 SDK namespace 추가
using BackEnd;
using System;

public class BackendLogin:MonoBehaviour
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

	// 구글 로그인 (뒤끝 툴킷 사용)
	public void GoogleLogin(Action onSuccess = null, Action<string> onFailure = null)
	{
#if UNITY_ANDROID
		TheBackend.ToolKit.GoogleLogin.Android.GoogleLogin((isSuccess, errorMessage, token) =>
		{
			if (!isSuccess)
			{
				Debug.LogError("구글 로그인 실패: " + errorMessage);
				onFailure?.Invoke(errorMessage);
				return;
			}

			Debug.Log("구글 토큰: " + token);
			var bro = Backend.BMember.AuthorizeFederation(token, FederationType.Google);

			if (bro.IsSuccess())
			{
				Debug.Log("구글 연동 로그인 성공: " + bro);
				onSuccess?.Invoke();
			}
			else
			{
				string error = bro.GetErrorCode() + " - " + bro.GetMessage();
				Debug.LogError("페데레이션 로그인 실패: " + error);
				onFailure?.Invoke(error);
			}
		});
#else
        Debug.LogError("구글 로그인은 Android에서만 지원됩니다.");
        onFailure?.Invoke("Android에서만 지원됩니다.");
#endif
	}

	// 구글 로그아웃 (계정 다시 선택하고 싶을 때)
	public void GoogleLogout()
	{
#if UNITY_ANDROID
		TheBackend.ToolKit.GoogleLogin.Android.GoogleSignOut((isSuccess, errorMessage) =>
		{
			if (!isSuccess)
				Debug.LogWarning("구글 로그아웃 실패: " + errorMessage);
			else
				Debug.Log(" 구글 로그아웃 성공");
		});
#endif
	}
}