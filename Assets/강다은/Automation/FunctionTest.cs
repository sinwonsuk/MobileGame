using UnityEngine;
using BackEnd;
using LitJson;
using System.Collections.Generic;
using System.IO;

public class FunctionTest : MonoBehaviour
{
	void Start()
	{
		//FoodTableData foodData = new FoodTableData();
		//foodData.foodName = "예시용";
		//foodData.foodPrice = 150;
		//StaticDataUploader.InsertStaticData(foodData.tableName, foodData.ToParam());
		//
		//FoodTableData foodData2 = new FoodTableData("버섯스프", 150);
		//StaticDataUploader.InsertStaticData(foodData2.tableName, foodData2.ToParam());
	}

	/*void TestInsertFromCsv()
	{

		// 1. CSV 경로 설정
		string path = @"C:\Users\daeun4001\OneDrive\Desktop\MobileGame\Assets\강다은\Automation\foods.csv";

		if (!File.Exists(path))
		{
			Debug.LogError($"CSV 파일이 존재하지 않음: {path}");
			return;
		}

		// 2. CSV 내용 읽기
		string[] lines = File.ReadAllLines(path);
		if (lines.Length < 2)
		{
			Debug.LogError("CSV 내용이 비어 있거나 헤더만 있음");
			return;
		}

		string[] headers = lines[0].Split(',');
		JsonData rows = new JsonData();
		rows.SetJsonType(JsonType.Array);

		// 3. 각 줄을 JsonData로 변환
		for (int i = 1; i < lines.Length; i++)
		{
			string[] values = lines[i].Split(',');
			if (values.Length != headers.Length)
			{
				Debug.LogWarning($"줄 {i}의 열 개수가 맞지 않음, 건너뜀");
				continue;
			}

			JsonData row = new JsonData();
			for (int j = 0; j < headers.Length; j++)
				row[headers[j]] = values[j];

			rows.Add(row);

			Debug.Log($"[CSV] row {i}: {row.ToJson()}");
		}

		// 4. Function 호출을 위한 body Json 구성
		JsonData body = new JsonData();
		body["table"] = "FOODS";
		body["rows"] = rows;

		Param param = new Param();
		param.Add("body", body.ToJson()); // ← 반드시 ToJson() 해야 문자열로 들어감

		Debug.Log($"[FunctionCall] 최종 요청 Json: {body.ToJson()}");

		var bro = Backend.BFunc.InvokeFunction("InsertFromCsv", param);
		Debug.Log($"statusCode: {bro.StatusCode}");
		Debug.Log($"IsSuccess: {bro.IsSuccess()}");
		Debug.Log($"raw JSON: {bro.GetReturnValuetoJSON()}");

		if (bro.IsSuccess())
		{
			Debug.Log("Function 호출 성공: " + bro.GetReturnValuetoJSON().ToJson());
		}
		else
		{
			Debug.LogError($"Function 호출 실패: {bro.GetStatusCode()} - {bro.GetMessage()}");
		}
	}*/
}
