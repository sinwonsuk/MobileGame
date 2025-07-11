using System.Collections.Generic;
using System.Text;
using UnityEngine;

// 뒤끝 SDK namespace 추가
using BackEnd;
using static UnityEngine.Rendering.DebugUI.Table;

public class UserData
{
	public string nickname = "default"; // null 비허용 대응
	public int reputation = 1;
	public float basicAtk = 3.5f;
	public string bio = "친추는 언제나 환영입니다.";
    public int gold = 0;
	public List<string> friends = new List<string>(); // 비워두면 됨

	public override string ToString()
    {
        StringBuilder result = new StringBuilder();
        result.AppendLine($"nickname : {nickname}");
		result.AppendLine($"level : {reputation}");
        result.AppendLine($"atk : {basicAtk}");
        result.AppendLine($"gold : {gold}");
        result.AppendLine($"bio : {bio}");

		foreach (var friend in friends)
		{
			result.AppendLine($"| {friend}");
		}

		return result.ToString();
    }
}

public class BackendGameData
{
    private static BackendGameData _instance = null;

    public static BackendGameData Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new BackendGameData();
            }

            return _instance;
        }
    }

    public static UserData userData;

    public string gameDataRowInDate = string.Empty;

    public void GameDataGetOrInsert(System.Action onSuccess = null)
    {
        Debug.Log("게임 정보 존재 여부 확인");

        Backend.GameData.GetMyData("USER_DATA", new Where(), (callback) =>
        {
			if (callback.IsSuccess())
			{
				Debug.Log("게임 정보 조회 성공");
                var rows = callback.FlattenRows();

				if (rows.Count > 0)
				{
					gameDataRowInDate = rows[0]["inDate"].ToString();
					ParseUserData(rows[0]);
				}
				else
				{
					Debug.LogWarning("조회 성공했지만 데이터 없음 → Insert");
					GameDataInsert(onSuccess);
					return;
				}

				onSuccess?.Invoke(); // 성공 콜백 호출
			}
			else
			{
				Debug.LogError("게임 정보 조회 실패: ");
                GameDataInsert(onSuccess);
			}
		});

	
	}

    private void GameDataInsert(System.Action onSuccess = null)
    {
        if (userData == null)
        {
            userData = new UserData();
        }

        Debug.Log("데이터를 초기화합니다.");
        //userData.level = 1;
        //userData.atk = 3.5f;
        //userData.info = "친추는 언제나 환영입니다.";
        //
		//userData.equipment.Add("전사의 투구");
        //userData.equipment.Add("강철 갑옷");
        //userData.equipment.Add("헤르메스의 군화");
        //
        //userData.inventory.Add("빨간포션", 1);
        //userData.inventory.Add("하얀포션", 1);
        //userData.inventory.Add("파란포션", 1);
        

		Debug.Log("뒤끝 업데이트 목록에 해당 데이터들을 추가합니다.");
        Param param = new Param();
        param.Add("nickname", userData.nickname);
		param.Add("reputation", userData.reputation);
        param.Add("basicAtk", userData.basicAtk);
        param.Add("bio", userData.bio);
        param.Add("gold", userData.gold);
        param.Add("friends", userData.friends);


		Debug.Log("게임 정보 데이터 삽입을 요청합니다.");
        var bro = Backend.GameData.Insert("USER_DATA", param);

        if (bro.IsSuccess())
        {
            Debug.Log("게임 정보 데이터 삽입에 성공했습니다. : " + bro);

            //삽입한 게임 정보의 고유값입니다.  
            gameDataRowInDate = bro.GetInDate();

            var getBro = Backend.GameData.GetMyData("USER_DATA", new Where());
            if(getBro.IsSuccess())
            {
                var rows = getBro.FlattenRows();
                if(rows.Count > 0)
                {
                    gameDataRowInDate = rows[0]["inDate"].ToString();
					ParseUserData(rows[0]); // 데이터 파싱
				}
			}
            onSuccess?.Invoke(); // 성공 콜백 호출
		}
        else
        {
            Debug.LogError("게임 정보 데이터 삽입에 실패했습니다. : " + bro);
        }
    }

    //public void GameDataGet()
    //{
    //    Debug.Log("게임 정보 조회 함수를 호출합니다.");
    //
    //    var bro = Backend.GameData.GetMyData("USER_DATA", new Where());
    //
    //    if (bro.IsSuccess())
    //    {
    //        Debug.Log("게임 정보 조회에 성공했습니다. : " + bro);
    //
    //        LitJson.JsonData gameDataJson = bro.FlattenRows(); // Json으로 리턴된 데이터를 받아옵니다.  
    //
    //        // 받아온 데이터의 갯수가 0이라면 데이터가 존재하지 않는 것입니다.  
    //        if (gameDataJson.Count <= 0)
    //        {
    //            Debug.LogWarning("데이터가 존재하지 않습니다.");
    //        }
    //        else
    //        {
    //            gameDataRowInDate = gameDataJson[0]["inDate"].ToString(); //불러온 게임 정보의 고유값입니다.  
    //
    //            userData = new UserData();
    //
    //            userData.level = int.Parse(gameDataJson[0]["level"].ToString());
    //            userData.atk = float.Parse(gameDataJson[0]["atk"].ToString());
    //            userData.info = gameDataJson[0]["info"].ToString();
    //
    //            foreach (string itemKey in gameDataJson[0]["inventory"].Keys)
    //            {
    //                userData.inventory.Add(itemKey, int.Parse(gameDataJson[0]["inventory"][itemKey].ToString()));
    //            }
    //
    //            foreach (LitJson.JsonData equip in gameDataJson[0]["equipment"])
    //            {
    //                userData.equipment.Add(equip.ToString());
    //            }
    //
    //            Debug.Log(userData.ToString());
    //        }
    //    }
    //    else
    //    {
    //        Debug.LogError("게임 정보 조회에 실패했습니다. : " + bro);
    //    }
    //}

    private void ParseUserData(LitJson.JsonData gameDataJson)
	{
        userData = new UserData
        {
            nickname = gameDataJson["nickname"].ToString(),
			reputation = int.Parse(gameDataJson["reputation"].ToString()),
            basicAtk = float.Parse(gameDataJson["basicAtk"].ToString()),
            bio = gameDataJson["bio"].ToString(),
			gold = int.Parse(gameDataJson["gold"].ToString()),
		};

		foreach (LitJson.JsonData friend in gameDataJson["friends"])
        {
            userData.friends.Add(friend.ToString());
		}

		Debug.Log("유저 데이터 로드 완료\n" + userData.ToString());
        
	}

	public void LevelUp()
    {
        Debug.Log("레벨을 1 증가시킵니다.");
        userData.reputation += 1;
        userData.basicAtk += 3.5f;
    }

    // 게임 정보 수정하기
    public void GameDataUpdate()
    {
        if (userData == null)
        {
            Debug.LogError("서버에서 다운받거나 새로 삽입한 데이터가 존재하지 않습니다. Insert 혹은 Get을 통해 데이터를 생성해주세요.");
            return;
        }

        Param param = new Param();
        param.Add("nickname", userData.nickname);
		param.Add("reputation", userData.reputation);
        param.Add("basicAtk", userData.basicAtk);
        param.Add("bio", userData.bio);
        param.Add("gold", userData.gold);
        param.Add("friends", userData.friends);

		BackendReturnObject bro = null;

        if (string.IsNullOrEmpty(gameDataRowInDate))
        {
            Debug.Log("내 제일 최신 게임 정보 데이터 수정을 요청합니다.");

            bro = Backend.GameData.Update("USER_DATA", new Where(), param);
        }
        else
        {
            Debug.Log($"{gameDataRowInDate}의 게임 정보 데이터 수정을 요청합니다.");

            bro = Backend.GameData.UpdateV2("USER_DATA", gameDataRowInDate, Backend.UserInDate, param);
        }

        if (bro.IsSuccess())
        {
            Debug.Log("게임 정보 데이터 수정에 성공했습니다. : " + bro);
        }
        else
        {
            Debug.LogError("게임 정보 데이터 수정에 실패했습니다. : " + bro);
        }
    }
}