using System.Collections.Generic;
using System.Text;
using UnityEngine;

// �ڳ� SDK namespace �߰�
using BackEnd;
using static UnityEngine.Rendering.DebugUI.Table;

public class UserData
{
	public string nickname = "default"; // null ����� ����
	public int level = 1;
	public float atk = 3.5f;
	public string info = "ģ�ߴ� ������ ȯ���Դϴ�.";
	public Dictionary<string, int> inventory = new Dictionary<string, int>();
	public List<string> equipment = new List<string>();
	public string friends = ""; // ����θ� ��

	// �����͸� ������ϱ� ���� �Լ��Դϴ�.(Debug.Log(UserData);)
	public override string ToString()
    {
        StringBuilder result = new StringBuilder();
        result.AppendLine($"nickname : {nickname}");
		result.AppendLine($"level : {level}");
        result.AppendLine($"atk : {atk}");
        result.AppendLine($"info : {info}");

        result.AppendLine($"inventory");
        foreach (var itemKey in inventory.Keys)
        {
            result.AppendLine($"| {itemKey} : {inventory[itemKey]}��");
        }

        result.AppendLine($"equipment");
        foreach (var equip in equipment)
        {
            result.AppendLine($"| {equip}");
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
        Debug.Log("���� ���� ���� ���� Ȯ��");

        Backend.GameData.GetMyData("USER_DATA", new Where(), (callback) =>
        {
			if (callback.IsSuccess())
			{
				Debug.Log("���� ���� ��ȸ ����");
                var rows = callback.FlattenRows();

				if (rows.Count > 0)
				{
					gameDataRowInDate = rows[0]["inDate"].ToString();
					ParseUserData(rows[0]);
				}
				else
				{
					Debug.LogWarning("��ȸ ���������� ������ ���� �� Insert");
					GameDataInsert(onSuccess);
					return;
				}

				onSuccess?.Invoke(); // ���� �ݹ� ȣ��
			}
			else
			{
				Debug.LogError("���� ���� ��ȸ ����: ");
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

        Debug.Log("�����͸� �ʱ�ȭ�մϴ�.");
        //userData.level = 1;
        //userData.atk = 3.5f;
        //userData.info = "ģ�ߴ� ������ ȯ���Դϴ�.";
        //
		//userData.equipment.Add("������ ����");
        //userData.equipment.Add("��ö ����");
        //userData.equipment.Add("�츣�޽��� ��ȭ");
        //
        //userData.inventory.Add("��������", 1);
        //userData.inventory.Add("�Ͼ�����", 1);
        //userData.inventory.Add("�Ķ�����", 1);
        

		Debug.Log("�ڳ� ������Ʈ ��Ͽ� �ش� �����͵��� �߰��մϴ�.");
        Param param = new Param();
        param.Add("nickname", userData.nickname);
		param.Add("level", userData.level);
        param.Add("atk", userData.atk);
        param.Add("info", userData.info);
        param.Add("equipment", userData.equipment);
        param.Add("inventory", userData.inventory);
        param.Add("friends", userData.friends);


		Debug.Log("���� ���� ������ ������ ��û�մϴ�.");
        var bro = Backend.GameData.Insert("USER_DATA", param);

        if (bro.IsSuccess())
        {
            Debug.Log("���� ���� ������ ���Կ� �����߽��ϴ�. : " + bro);

            //������ ���� ������ �������Դϴ�.  
            gameDataRowInDate = bro.GetInDate();

            var getBro = Backend.GameData.GetMyData("USER_DATA", new Where());
            if(getBro.IsSuccess())
            {
                var rows = getBro.FlattenRows();
                if(rows.Count > 0)
                {
                    gameDataRowInDate = rows[0]["inDate"].ToString();
					ParseUserData(rows[0]); // ������ �Ľ�
				}
			}
            onSuccess?.Invoke(); // ���� �ݹ� ȣ��
		}
        else
        {
            Debug.LogError("���� ���� ������ ���Կ� �����߽��ϴ�. : " + bro);
        }
    }

    //public void GameDataGet()
    //{
    //    Debug.Log("���� ���� ��ȸ �Լ��� ȣ���մϴ�.");
    //
    //    var bro = Backend.GameData.GetMyData("USER_DATA", new Where());
    //
    //    if (bro.IsSuccess())
    //    {
    //        Debug.Log("���� ���� ��ȸ�� �����߽��ϴ�. : " + bro);
    //
    //        LitJson.JsonData gameDataJson = bro.FlattenRows(); // Json���� ���ϵ� �����͸� �޾ƿɴϴ�.  
    //
    //        // �޾ƿ� �������� ������ 0�̶�� �����Ͱ� �������� �ʴ� ���Դϴ�.  
    //        if (gameDataJson.Count <= 0)
    //        {
    //            Debug.LogWarning("�����Ͱ� �������� �ʽ��ϴ�.");
    //        }
    //        else
    //        {
    //            gameDataRowInDate = gameDataJson[0]["inDate"].ToString(); //�ҷ��� ���� ������ �������Դϴ�.  
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
    //        Debug.LogError("���� ���� ��ȸ�� �����߽��ϴ�. : " + bro);
    //    }
    //}

    private void ParseUserData(LitJson.JsonData gameDataJson)
	{
        userData = new UserData
        {
            nickname = gameDataJson["nickname"].ToString(),
			level = int.Parse(gameDataJson["level"].ToString()),
            atk = float.Parse(gameDataJson["atk"].ToString()),
            info = gameDataJson["info"].ToString(),
			friends = gameDataJson["friends"].ToString()
		};

        userData.inventory.Clear();

		foreach (string itemKey in gameDataJson["inventory"].Keys)
		{
			userData.inventory.Add(itemKey, int.Parse(gameDataJson["inventory"][itemKey].ToString()));
		}
		foreach (LitJson.JsonData equip in gameDataJson["equipment"])
		{
			userData.equipment.Add(equip.ToString());
		}

		Debug.Log("���� ������ �ε� �Ϸ�\n" + userData.ToString());
        
	}

	public void LevelUp()
    {
        Debug.Log("������ 1 ������ŵ�ϴ�.");
        userData.level += 1;
        userData.atk += 3.5f;
        userData.info = "������ �����մϴ�.";
    }

    // ���� ���� �����ϱ�
    public void GameDataUpdate()
    {
        if (userData == null)
        {
            Debug.LogError("�������� �ٿ�ްų� ���� ������ �����Ͱ� �������� �ʽ��ϴ�. Insert Ȥ�� Get�� ���� �����͸� �������ּ���.");
            return;
        }

        Param param = new Param();
        param.Add("nickname", userData.nickname);
		param.Add("level", userData.level);
        param.Add("atk", userData.atk);
        param.Add("info", userData.info);
        param.Add("equipment", userData.equipment);
        param.Add("inventory", userData.inventory);
        param.Add("friends", userData.friends);

		BackendReturnObject bro = null;

        if (string.IsNullOrEmpty(gameDataRowInDate))
        {
            Debug.Log("�� ���� �ֽ� ���� ���� ������ ������ ��û�մϴ�.");

            bro = Backend.GameData.Update("USER_DATA", new Where(), param);
        }
        else
        {
            Debug.Log($"{gameDataRowInDate}�� ���� ���� ������ ������ ��û�մϴ�.");

            bro = Backend.GameData.UpdateV2("USER_DATA", gameDataRowInDate, Backend.UserInDate, param);
        }

        if (bro.IsSuccess())
        {
            Debug.Log("���� ���� ������ ������ �����߽��ϴ�. : " + bro);
        }
        else
        {
            Debug.LogError("���� ���� ������ ������ �����߽��ϴ�. : " + bro);
        }
    }
}