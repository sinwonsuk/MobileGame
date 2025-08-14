// 뒤끝 SDK namespace 추가
using BackEnd;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

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

	private UserData lastSnapshot;

	public void TakeSnapshot()
	{
		lastSnapshot = new UserData
		{
			nickname = this.nickname,
			reputation = this.reputation,
			basicAtk = this.basicAtk,
			bio = this.bio,
			gold = this.gold,
			friends = new List<string>(this.friends)
		};
	}

	public bool HasChanged()
	{
		if (lastSnapshot == null) return true;

		return nickname != lastSnapshot.nickname ||
			   reputation != lastSnapshot.reputation ||
			   basicAtk != lastSnapshot.basicAtk ||
			   bio != lastSnapshot.bio ||
			   gold != lastSnapshot.gold ||
			   !Enumerable.SequenceEqual(friends, lastSnapshot.friends);
	}
}

public class BackendGameData : MonoBehaviour, IAutoSavable
{
	public static BackendGameData Instance { get; private set; }

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);  
		}
		else
		{
			Destroy(gameObject); 
		}
	}


	public UserData userData { get; private set; }

	public string gameDataRowInDate = string.Empty;

	public event System.Action OnUserDataReady;

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

					AutoSaveManager.Instance?.RegisterAutoSavable(this);

					onSuccess?.Invoke();       // 성공 콜백 호출
                    OnUserDataReady?.Invoke(); // 데이터 준비 완료 이벤트 호출
				}
				else
				{
					Debug.LogWarning("조회 성공했지만 데이터 없음 → Insert");
					GameDataInsert(onSuccess);
				}

			}
			else
			{
				Debug.LogError("게임 정보 조회 실패: ");
                GameDataInsert(onSuccess);
			}
		});

	
	}

	public void Initialized()
	{
		if(userData != null)
		{
			userData = new UserData
			{
				nickname = Backend.UserNickName,
				reputation = 1,
				basicAtk = 3.5f,
				bio = "친추는 언제나 환영입니다.",
				gold = 0,
				friends = new List<string>()
			};

			userData.TakeSnapshot();
		}
		else
		{
			Debug.LogWarning("userData가 null입니다. 초기화되지 않았습니다.");
		}
	}

	private void GameDataInsert(System.Action onSuccess = null)
    {
        if (userData == null)
        {
            userData = new UserData();
        }

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

			this.userData = new UserData
			{
				nickname = userData.nickname,
				reputation = userData.reputation,
				basicAtk = userData.basicAtk,
				bio = userData.bio,
				gold = userData.gold,
				friends = new List<string>(userData.friends)
			};

			AutoSaveManager.Instance?.RegisterAutoSavable(this);

			onSuccess?.Invoke(); // 성공 콜백 호출
			OnUserDataReady?.Invoke();
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
            nickname = Backend.UserNickName,
			reputation = int.Parse(gameDataJson["reputation"].ToString()),
            basicAtk = float.Parse(gameDataJson["basicAtk"].ToString()),
            bio = gameDataJson["bio"].ToString(),
			gold = int.Parse(gameDataJson["gold"].ToString()),
			friends = new List<string>()
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
    public void GameDataUpdate(System.Action onComplete = null)
    {
		if (userData == null)
		{
			Debug.LogWarning("[GameDataUpdate] userData가 null입니다. 업데이트 생략");
			return;
		}

		Param param = new Param();
        param.Add("nickname", Backend.UserNickName);
		param.Add("reputation", userData.reputation);
        param.Add("basicAtk", userData.basicAtk);
        param.Add("bio", userData.bio);
        param.Add("gold", userData.gold);
        param.Add("friends", userData.friends);

        if (string.IsNullOrEmpty(gameDataRowInDate))
        {
            Debug.Log("제일 최신 게임 정보 데이터 수정을 요청합니다.");

			Backend.GameData.Update("USER_DATA", new Where(), param, bro =>
			{
				if (bro.IsSuccess())
					Debug.Log("자동 저장 완료 / 게임 정보 수정 완료 : " + bro);
				else
					Debug.LogError("게임 정보 수정 실패 : " + bro);

				onComplete?.Invoke();
			});
		}
		else
		{
			Debug.Log($"{gameDataRowInDate}의 게임 정보 데이터 수정을 요청합니다.");

			Backend.GameData.UpdateV2("USER_DATA", gameDataRowInDate, Backend.UserInDate, param, bro =>
			{
				if (bro.IsSuccess())
					Debug.Log("자동 저장 완료 / 게임 정보 수정 완료 : " + bro);
				else
					Debug.LogError("게임 정보 수정 실패 : " + bro);

				onComplete?.Invoke();
			});
		}
    }
	public void AutoSave()
	{
		if (userData.HasChanged())
		{
			GameDataUpdate();
			userData.TakeSnapshot();
		}
		else
		{
			Debug.Log("유저 정보 변경 사항 없음 -> 저장 생략");
		}
	}
}