using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BackEnd;
using System;
using System.Text.RegularExpressions;

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

		// 1. 서버 데이터 로드
		Dictionary<string, LitJson.JsonData> serverDataMap = new();
		yield return LoadAllServerData(tableName, keyColumns, serverDataMap);

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
			string line = lines[i].Trim(_trimChars);
			if (string.IsNullOrEmpty(line)) continue;

			string[] values = line.Split(',').Select(s => s.Trim()).ToArray();
			if (values.Length != headers.Length)
			{
				Debug.LogWarning($"[SKIP] 열 수 불일치: {line}");
				continue;
			}

			string key = CreateCompositeKey(headers, values, keyColumns);
			if (!csvKeys.Add(key))
			{
				Debug.LogWarning($"[SKIP DUP] 같은 CSV 안에서 중복 키: {key}");
				continue;
			}

			Param param = new Param();
			for (int j = 0; j < headers.Length; j++)
			{
				string h = headers[j];
				string v = values[j];
				if (int.TryParse(v, out int intVal)) param.Add(h, intVal);
				else if (float.TryParse(v, out float floatVal)) param.Add(h, floatVal);
				else param.Add(h, v);
			}

			// 3. 삽입 또는 갱신
			if (!serverDataMap.ContainsKey(key))
			{
				bool inserted = false;
				yield return StaticDataUploader.InsertStaticDataAsync(tableName, param, success =>
				{
					inserted = success;
				});

				if (inserted)
				{
					// Insert 후 해당 key로 서버 다시 조회
					bool updated = false;
					Backend.GameData.Get(tableName, new Where(), callback =>
					{
						if (callback.IsSuccess())
						{
							foreach (LitJson.JsonData row in callback.FlattenRows())
							{
								string updatedKey = CreateCompositeKey(row, keyColumns);
								serverDataMap[updatedKey] = row;
							}
						}
						else
						{
							Debug.LogWarning($"[WARNING] 삽입 후 재조회 실패: {callback}");
						}
						updated = true;
					});
					yield return new WaitUntil(() => updated);
				}
			}
			else if (IsRowDifferent(serverDataMap[key], param, key))
			{
				string inDate = serverDataMap[key]["inDate"].ToString();
				var bro = Backend.GameData.UpdateV2(tableName, inDate, Backend.UserInDate, param);
				Debug.Log(bro.IsSuccess() ? $"[UPDATE SUCCESS] {tableName} : {key}" : $"[UPDATE FAIL] {tableName} : {key} → {bro.GetMessage()}");
			}
			else
			{
				Debug.Log($"[SKIP] 동일 데이터: {key}");
			}
		}

		// 4. CSV에 없는 항목 삭제
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
				var bro = Backend.GameData.DeleteV2(tableName, inDate, Backend.UserInDate);
				Debug.Log(bro.IsSuccess() ? $"[DELETE SUCCESS] {tableName} : {key}" : $"[DELETE FAIL] {tableName} : {key} → {bro.GetMessage()}");
			}
		}
	}

	IEnumerator LoadAllServerData(string tableName, List<string> keyColumns, Dictionary<string, LitJson.JsonData> serverDataMap)
	{
		string offset = "";
		int limit = 100;
		bool isEnd = false;

		while (!isEnd)
		{
			bool isDone = false;
			BackendReturnObject bro = null;

			Backend.GameData.Get(tableName, new Where(), limit, offset, callback =>
			{
				bro = callback;
				isDone = true;
			});
			yield return new WaitUntil(() => isDone);

			if (bro == null || !bro.IsSuccess())
			{
				Debug.LogError($"[ERROR] {tableName} 서버 데이터 로드 실패: {bro}");
				yield break;
			}

			foreach (LitJson.JsonData row in bro.FlattenRows())
			{
				string key = CreateCompositeKey(row, keyColumns);
				serverDataMap[key] = row;
			}

			//  JSON 수동 파싱해서 offset 추출
			try
			{
				var json = LitJson.JsonMapper.ToObject(bro.GetReturnValue());
				if (json.ContainsKey("offset"))
				{
					offset = json["offset"].ToString();
				}
				else
				{
					isEnd = true; // offset 없으면 더 이상 없음
				}
			}
			catch (Exception e)
			{
				Debug.LogWarning($"[WARN] offset 파싱 실패 → 종료 처리: {e.Message}");
				isEnd = true;
			}
		}
	}



	// --- 유틸 함수 ---
	static string CleanCollapse(string s)
	{
		if (string.IsNullOrEmpty(s)) return "";
		s = s.Trim(_trimChars)
			.Replace('\u00A0', ' ')
			.Replace('　', ' ');
		return _multiSpace.Replace(s, " ");
	}
	static string Clean(string s) => CleanCollapse(s);
	static string JsonToPlain(LitJson.JsonData d) => d == null ? "" : CleanCollapse(d.ToString());

	string CreateCompositeKey(LitJson.JsonData row, List<string> cols) =>
		string.Join("|", cols.Select(c => row.ContainsKey(c) ? CleanCollapse(row[c].ToString()) : "null"));

	string CreateCompositeKey(string[] heads, string[] vals, List<string> cols) =>
		string.Join("|", cols.Select(c =>
		{
			int idx = Array.IndexOf(heads, c);
			return idx >= 0 ? CleanCollapse(vals[idx]) : "null";
		}));

	bool IsRowDifferent(LitJson.JsonData serverRow, Param csvParam, string debugKey)
	{
		foreach (DictionaryEntry kvp in (SortedList)csvParam.GetValue())
		{
			string col = (string)kvp.Key;
			if (col == "inDate") continue;

			string csvVal = Clean(kvp.Value?.ToString());
			string srvVal = serverRow.ContainsKey(col) ? JsonToPlain(serverRow[col]) : "";

			if (csvVal != srvVal)
			{
				Debug.LogWarning($"[DIFF] {debugKey} | {col} CSV='{csvVal}' SERVER='{srvVal}'");
				return true;
			}
		}
		return false;
	}

	// --- 필드 ---
	static readonly char[] _trimChars = { ' ', '\t', '\r', '\n', '\uFEFF' };
	static readonly Regex _multiSpace = new(@"[\u00A0\u3000 ]{2,}", RegexOptions.Compiled);
	private readonly HashSet<string> pendingKeys = new();

	[SerializeField]
	private List<string> tableFileNames = new List<string>
	{
		"FOODS", "FURNITURES", "FOOD_INGREDIENTS", "INGREDIENTS",
		"EQUIPMENTS", "EQUIPMENT_EFFECTS", "EMPLOYEE_MASTER", "FOOD_GRADES"
	};

	private Dictionary<string, List<string>> uniqueKeyColumnsMap = new()
	{
		{ "FOODS", new List<string> { "foodName" } },
		{ "FURNITURES", new List<string> { "furnitureName" } },
		{ "FOOD_INGREDIENTS", new List<string> { "foodIndate", "ingredientIndate" } },
		{ "INGREDIENTS", new List<string> { "ingredientName" } },
		{ "EQUIPMENTS", new List<string> { "equipmentName" } },
		{ "EQUIPMENT_EFFECTS", new List<string> { "effectIndate" } },
		{ "EMPLOYEE_MASTER", new List<string> { "employeeName" } },
		{ "FOOD_GRADES", new List<string> { "foodIndate", "grade" } }
	};
}
