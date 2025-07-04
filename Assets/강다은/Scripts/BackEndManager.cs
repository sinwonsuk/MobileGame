using UnityEngine;

// 뒤끝 SDK namespace 추가
using BackEnd;

public class BackendManager : MonoBehaviour
{
    void Start()
    {
        var bro = Backend.Initialize(); // 뒤끝 초기화

        // 뒤끝 초기화에 대한 응답값
        if (bro.IsSuccess())
        {
            Debug.Log("초기화 성공 : " + bro); // 성공일 경우 statusCode 204 Success
        }
        else
        {
            Debug.LogError("초기화 실패 : " + bro); // 실패일 경우 statusCode 400대 에러 발생
        }

        UserLogin("user1", "1234"); // [추가] 뒤끝 로그인 함수 호출
	}

    void Test()
    {
        //BackendLogin.Instance.CustomSignUp("user1", "1234"); // [추가] 뒤끝 회원가입 함수
        BackendLogin.Instance.CustomLogin("user1", "1234"); // [추가] 뒤끝 로그인
        //BackendLogin.Instance.UpdateNickname("원하는 이름"); // [추가] 닉네임 변겅

        //BackendGameData.Instance.GameDataInsert();

        BackendGameData.Instance.GameDataGet(); //[추가] 데이터 불러오기 함수
        Debug.Log("테스트를 종료합니다.");
    }

    void UserLogin(string id, string pw)
	{
		BackendLogin.Instance.CustomLogin(id, pw, () =>
		{
            BackendLogin.Instance.CustomLogin(id, pw, () =>
            {
                Debug.Log("로그인 성공 후 데이터 불러오기 시도");
                BackendGameData.Instance.GameDataGet(); // 로그인 성공 후 데이터 불러오기
            });

		});
	}
}