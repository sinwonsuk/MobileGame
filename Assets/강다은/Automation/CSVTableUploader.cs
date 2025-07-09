using UnityEngine;
using System.Collections.Generic;
using System.IO;
using BackEnd;
using System.Collections;
using static BackEnd.Quobject.SocketIoClientDotNet.Parser.Parser.Encoder;

public class CSVTableUploader : MonoBehaviour
{
	IEnumerator Start()
	{
		foreach (string tableName in tableFileNames)
		{
			yield return LoadExistingKeys(tableName);
			UploadCsvForTable(tableName);
		}
	}

	void UploadCsvForTable(string tableName)
	{
		TextAsset csvFile = Resources.Load<TextAsset>("CSVData/" + tableName);
		if (csvFile == null)
		{
			Debug.LogError($"[ERROR] Resources/CSVData/{tableName}.csv 파일 없음");
			return;
		}

		if (!uniqueKeyColumnsMap.TryGetValue(tableName, out List<string> keyColumns))
		{
			Debug.LogError($"[ERROR] {tableName} 고유 키 컬럼 설정 안 됨");
			return;
		}

		string[] lines = csvFile.text.Split('\n');
		if (lines.Length < 2) return;

		string[] headers = lines[0].Trim().Split(',');

		for (int i = 1; i < lines.Length; i++)
		{
			string line = lines[i].Trim();
			if (string.IsNullOrEmpty(line)) continue;

			string[] values = line.Split(',');
			if (values.Length != headers.Length)
			{
				Debug.LogWarning($"[SKIP] {tableName} - 열 수 불일치: {line}");
				continue;
			}

			string compositeKey = CreateCompositeKey(headers, values, keyColumns);

			if (existingKeys.TryGetValue(tableName, out var keys) && keys.Contains(compositeKey))
			{
				Debug.Log($"[SKIP] {tableName} - 중복 스킵: {compositeKey}");
				continue;
			}

			// Param 구성
			Param param = new Param();
			for (int j = 0; j < headers.Length; j++)
			{
				string key = headers[j].Trim();
				string value = values[j].Trim();

				if (int.TryParse(value, out int intVal))
					param.Add(key, intVal);
				else if (float.TryParse(value, out float floatVal))
					param.Add(key, floatVal);
				else
					param.Add(key, value);
			}

			StaticDataUploader.InsertStaticData(tableName, param);
			Debug.Log($"[INSERT] {tableName} > {param}");
		}
	}


	IEnumerator LoadExistingKeys(string tableName)
	{
		existingKeys[tableName] = new HashSet<string>();

		if (!uniqueKeyColumnsMap.TryGetValue(tableName, out List<string> keyColumns))
		{
			Debug.LogError($"[ERROR] {tableName} 고유 키 컬럼 설정 안 됨");
			yield break;
		}

		bool isDone = false;

		Backend.GameData.Get(tableName, new Where(), callback =>
		{
			if (callback.IsSuccess())
			{
				foreach (LitJson.JsonData row in callback.FlattenRows())
				{
					string compositeKey = CreateCompositeKey(row, keyColumns);
					existingKeys[tableName].Add(compositeKey);
				}
			}
			else
			{
				Debug.LogWarning($"[WARNING] {tableName} 키 불러오기 실패: {callback}");
			}

			isDone = true;
		});

		// 결과가 도착할 때까지 기다림
		yield return new WaitUntil(() => isDone);
	}


	// 중복 체크용 복합 키 생성 함수
	string CreateCompositeKey(LitJson.JsonData row, List<string> keyColumns)
	{
		List<string> parts = new List<string>();
		foreach (var col in keyColumns)
		{
			if (row.ContainsKey(col))
				parts.Add(row[col].ToString());
			else
				parts.Add("null"); // 또는 에러 처리
		}
		return string.Join("|", parts); // ex: "1|12"
	}

	string CreateCompositeKey(string[] headers, string[] values, List<string> keyColumns)
	{
		List<string> parts = new List<string>();
		foreach (var col in keyColumns)
		{
			int index = System.Array.IndexOf(headers, col);
			if (index >= 0 && index < values.Length)
				parts.Add(values[index].Trim());
			else
				parts.Add("null");
		}
		return string.Join("|", parts);
	}


	// 테이블마다 중복 체크용 고유 키 컬럼 이름을 설정
	private Dictionary<string, List<string>> uniqueKeyColumnsMap = new Dictionary<string, List<string>>
	{
		{ "FOODS", new List<string> { "foodName" } },
		{ "FURNITURES", new List<string> { "furnitureName" } },
		{ "FOOD_INGREDIENTS", new List<string> { "foodIndate", "ingredientIndate" } }, // 복합 키
		{ "INGREDIENTS", new List<string> { "ingredientName" } },
		{ "EQUIPMENTS", new List<string> { "equipmentName" } },
		{ "EQUIPMENT_EFFECTS", new List<string> { "effectIndate" } },
		{ "EMPLOYEE_MASTER", new List<string> { "employeeName" } },
	};


	private Dictionary<string, HashSet<string>> existingKeys = new Dictionary<string, HashSet<string>>();

	[SerializeField] private List<string> tableFileNames = new List<string> { "FOODS", "FURNITURES", "FOOD_INGREDIENTS", "INGREDIENTS", "EQUIPMENTS", "EQUIPMENT_EFFECTS", "EMPLOYEE_MASTER" };

}
