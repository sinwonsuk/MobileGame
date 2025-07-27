using BackEnd;
using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StaticDataInitializer : MonoBehaviour
{
	public IEnumerator InitializeAllStaticData()
	{
		yield return StartCoroutine(LoadTableData(
			tableName: "INGREDIENTS",
			applyAction: (row) =>
			{
				string indate = row["inDate"].ToString();
				var target = ingredientDataList.FirstOrDefault(i => i.indate == indate);
				if (target != null)
				{
					target.ingredientName = row["ingredientName"].ToString();
					target.ingredientPrice = int.Parse(row["ingredientPrice"].ToString());
					Debug.Log($"[INGREDIENT] 초기화 완료: {target.ingredientName}");
				}
			}));

		yield return StartCoroutine(LoadTableData(
			tableName: "FOODS",
			applyAction: (row) =>
			{
				string indate = row["inDate"].ToString();
				var target = foodDataList.FirstOrDefault(f => f.indate == indate);
				if (target != null)
				{
					target.displayName = row["foodName"].ToString();
					target.price = int.Parse(row["foodPrice"].ToString());
					target.cookingTime = float.Parse(row["cookingTime"].ToString());
					//target.baseGrade = int.Parse(row["baseGrade"].ToString());
					Debug.Log($"[FOOD] 초기화 완료: {target.displayName}");
				}
			}));

		yield return StartCoroutine(LoadTableData(
		tableName: "EMPLOYEE_MASTER",
		applyAction: (row) =>
		{
			string indate = row["inDate"].ToString();
			var target = employeeDataList.FirstOrDefault(e => e.indate == indate);
			if (target != null)
			{
				target.displayName = row["employeeName"].ToString();
				target.baseSalary = int.Parse(row["baseSalary"].ToString());
				target.basic_attack_Power = double.Parse(row["baseAtk"].ToString());
				target.basic_attack_Speed = double.Parse(row["baseAtkSpeed"].ToString());
				target.basictimer = double.Parse(row["timer"].ToString());
				target.basiccooltime = double.Parse(row["cooltime"].ToString());
				target.explain = row["explain"].ToString();

				if (Enum.TryParse(row["staffType"].ToString(), out StaffType type))
				{
					target.staffType = type;
				}
				else
				{
					Debug.LogWarning($"[EMPLOYEE] staffType 파싱 실패: {row["staffType"]}");
				}

				Debug.Log($"[EMPLOYEE] 초기화 완료: {target.displayName}");
			}
		}));

	}

	IEnumerator LoadTableData(string tableName, System.Action<LitJson.JsonData> applyAction)
	{
		string firstKey = null;
		const int limit = 100;
		bool isEnd = false;

		while (!isEnd)
		{
			bool isDone = false;
			BackendReturnObject bro = null;

			// 첫 요청이면 firstKey 없이
			if (string.IsNullOrEmpty(firstKey))
			{
				Backend.GameData.Get(tableName, new Where(), limit, callback =>
				{
					bro = callback;
					isDone = true;
				});
			}
			else
			{
				// firstKey로 이어받기
				Backend.GameData.Get(tableName, new Where(), limit, firstKey, callback =>
				{
					bro = callback;
					isDone = true;
				});
			}

			yield return new WaitUntil(() => isDone);

			if (!bro.IsSuccess())
			{
				Debug.LogError($"[{tableName}] 테이블 불러오기 실패: {bro.GetMessage()}");
				yield break;
			}

			var rows = bro.FlattenRows();
			foreach (LitJson.JsonData row in rows)
			{
				applyAction(row);
			}

			// 다음 firstKey 추출
			try
			{
				var json = LitJson.JsonMapper.ToObject(bro.GetReturnValue());
				if (json.ContainsKey("firstKey") && json["firstKey"] != null)
				{
					firstKey = json["firstKey"]["inDate"]["S"].ToString();
				}
				else
				{
					isEnd = true;
				}
			}
			catch (Exception e)
			{
				Debug.LogWarning($"[WARN] firstKey 파싱 실패 → 종료 처리: {e.Message}");
				isEnd = true;
			}
		}
	}


	[SerializeField] private List<IngredientData> ingredientDataList;
	[SerializeField] private List<FoodData> foodDataList;
	[SerializeField] private List<StaffStatsSO> employeeDataList;
}
