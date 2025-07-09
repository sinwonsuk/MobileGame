using UnityEngine;
using System.Collections.Generic;
using System.IO;
using BackEnd;
using System.Collections;
using System.Linq;

public class CSVTableUploader : MonoBehaviour
{
	IEnumerator Start()
	{
		foreach (string tableName in tableFileNames)
		{
			yield return SyncTableWithCsv(tableName);
		}
	}
	IEnumerator SyncTableWithCsv(string tableName)
	{
		if (!uniqueKeyColumnsMap.TryGetValue(tableName, out var keyColumns))
		{
			Debug.LogError($"[ERROR] {tableName} 고유 키 설정이 없습니다.");
			yield break;
		}

		// 1. 서버 데이터 불러오기
		Dictionary<string, LitJson.JsonData> serverDataMap = new();
		bool isDone = false;

		Backend.GameData.Get(tableName, new Where(), callback =>
		{
			if (callback.IsSuccess())
			{
				foreach (LitJson.JsonData row in callback.FlattenRows())
				{
					string key = CreateCompositeKey(row, keyColumns);
					serverDataMap[key] = row;
				}
			}
			else
			{
				Debug.LogWarning($"[WARNING] 서버 데이터 로드 실패: {callback}");
			}
			isDone = true;
		});
		yield return new WaitUntil(() => isDone);

		// 2. CSV 로드
		TextAsset csvFile = Resources.Load<TextAsset>("CSVData/" + tableName);
		if (csvFile == null)
		{
			Debug.LogError($"[ERROR] Resources/CSVData/{tableName}.csv 파일이 존재하지 않습니다.");
			yield break;
		}

		string[] lines = csvFile.text.Split('\n');
		if (lines.Length < 2) yield break;

		string[] headers = lines[0].Trim().Split(',');

		HashSet<string> csvKeys = new();

		for (int i = 1; i < lines.Length; i++)
		{
			string line = lines[i].Trim();
			if (string.IsNullOrEmpty(line)) continue;

			string[] values = line.Split(',');
			if (values.Length != headers.Length)
			{
				Debug.LogWarning($"[SKIP] 열 수 불일치: {line}");
				continue;
			}

			string key = CreateCompositeKey(headers, values, keyColumns);
			csvKeys.Add(key);

			Param param = new Param();
			for (int j = 0; j < headers.Length; j++)
			{
				string h = headers[j].Trim();
				string v = values[j].Trim();
				if (int.TryParse(v, out int intVal)) param.Add(h, intVal);
				else if (float.TryParse(v, out float floatVal)) param.Add(h, floatVal);
				else param.Add(h, v);
			}

			if (!serverDataMap.ContainsKey(key))
			{
				// 삽입
				StaticDataUploader.InsertStaticData(tableName, param);
				Debug.Log($"[INSERT] {tableName} : {key}");
			}
			else if (IsRowDifferent(serverDataMap[key], param))
			{
				// 수정
				string inDate = serverDataMap[key]["inDate"].ToString();

				BackendReturnObject bro = Backend.GameData.UpdateV2(tableName, inDate, Backend.UserInDate, param);

				if (bro.IsSuccess())
					Debug.Log($"[UPDATE SUCCESS] {tableName} : {key} + inDate = {inDate}");
				else
					Debug.LogWarning($"[UPDATE FAIL] {tableName} : {key} + {bro.GetMessage()}");

			}
			else
			{
				// 동일: 무시
				Debug.Log($"[SKIP] 동일 데이터: {key}");
			}
		}

		// 3. CSV에 없는 데이터는 삭제
		foreach (var key in serverDataMap.Keys)
		{
			if (!csvKeys.Contains(key))
			{
				if (!serverDataMap[key].ContainsKey("inDate"))
				{
					Debug.LogError($"[DELETE ERROR] {tableName} : {key} → inDate 없음");
					continue;
				}

				string inDate = serverDataMap[key]["inDate"].ToString();
				Debug.Log($"[DELETE ATTEMPT] {tableName} : {key} → inDate = {inDate}");

				var bro = Backend.GameData.DeleteV2(tableName, inDate, Backend.UserInDate);

				if (bro.IsSuccess())
					Debug.Log($"[DELETE SUCCESS] {tableName} : {key}");
				else
					Debug.LogWarning($"[DELETE FAIL] {tableName} : {key} → {bro.GetMessage()}");
			}
		}


	}

	string CreateCompositeKey(LitJson.JsonData row, List<string> keyColumns)
	{
		List<string> parts = new();
		foreach (var col in keyColumns)
			parts.Add(row.ContainsKey(col) ? row[col].ToString() : "null");
		return string.Join("|", parts);
	}

	string CreateCompositeKey(string[] headers, string[] values, List<string> keyColumns)
	{
		List<string> parts = new();
		foreach (var col in keyColumns)
		{
			int idx = System.Array.IndexOf(headers, col);
			parts.Add((idx >= 0 && idx < values.Length) ? values[idx].Trim() : "null");
		}
		return string.Join("|", parts);
	}

	bool IsRowDifferent(LitJson.JsonData serverRow, Param csvParam)
	{
		SortedList csvDict;
		try
		{
			csvDict = csvParam.GetValue() as SortedList;
			if (csvDict == null)
			{
				Debug.LogError("csvParam을 SortedList로 변환 실패");
				return true;
			}
		}
		catch
		{
			Debug.LogError("csvParam.GetValue() 변환 중 예외 발생");
			return true;
		}

		foreach (DictionaryEntry kvp in csvDict)
		{
			string key = kvp.Key.ToString();

			if (!serverRow.ContainsKey(key))
				return true;

			string csvValueStr = kvp.Value?.ToString()?.Trim() ?? "";
			string serverValueStr = serverRow[key]?.ToString()?.Trim() ?? "";

			if (!string.Equals(csvValueStr, serverValueStr, System.StringComparison.OrdinalIgnoreCase))
				return true;
		}

		return false;
	}


	// 테이블마다 중복 체크용 고유 키 컬럼 설정
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
