using BackEnd;
using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StaticDataInitializer : MonoBehaviour
{
	[SerializeField] private List<IngredientData> ingredientDataList;
	[SerializeField] private List<FoodData> foodDataList;
	[SerializeField] private List<StaffStatsSO> employeeDataList;

	public IEnumerator InitializeAllStaticData()
	{
		yield return StartCoroutine(LoadTableData(
			"INGREDIENTS",
			row =>
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
		yield return new WaitForSeconds(0.2f);

		yield return StartCoroutine(LoadTableData(
			"FOODS",
			row =>
			{
				string indate = row["inDate"].ToString();
				var target = foodDataList.FirstOrDefault(f => f.indate == indate);
				if (target != null)
				{
					target.displayName = row["foodName"].ToString();
					target.price = int.Parse(row["foodPrice"].ToString());
					target.cookingTime = float.Parse(row["cookingTime"].ToString());
					Debug.Log($"[FOOD] 초기화 완료: {target.displayName}");
				}
			}));
		yield return new WaitForSeconds(0.2f);

		// FOOD_GRADES + FOOD_ENHANCE_MATERIAL
		Dictionary<string, EnhanceStepData> stepDataMap = new();

		yield return StartCoroutine(LoadTableData("FOOD_GRADES", row =>
		{
			string enhanceIndate = row["inDate"].ToString();
			var stepData = new EnhanceStepData
			{
				indate = enhanceIndate,
				foodIndate = row["foodIndate"].ToString(),
				step = int.Parse(row["grade"].ToString()),
				cost = int.Parse(row["goldCost"].ToString()),
				successRate = float.Parse(row["successRate"].ToString()),
				ingredients = new List<EnhanceMaterialData>()
			};
			stepDataMap[enhanceIndate] = stepData;
		}));
		yield return new WaitForSeconds(0.2f);

		yield return StartCoroutine(LoadTableData("FOOD_ENHANCE_MATERIAL", row =>
		{
			string enhanceIndate = row["foodEnhanceIndate"].ToString();
			if (stepDataMap.TryGetValue(enhanceIndate, out var stepData))
			{
				var material = new EnhanceMaterialData
				{
					indate = row["ingredientIndate"].ToString(),
					name = row["ingredientName"].ToString(),
					quantity = int.Parse(row["quantity"].ToString())
				};
				stepData.ingredients.Add(material);
			}
			else
			{
				Debug.LogWarning($"[WARN] stepDataMap에 {enhanceIndate} 없음");
			}
		}));
		yield return new WaitForSeconds(0.2f);

		// 연결
		foreach (var food in foodDataList)
		{
			food.enhanceSteps = stepDataMap.Values
				.Where(step => step.foodIndate == food.indate)
				.OrderBy(step => step.step)
				.ToList();
		}

		yield return StartCoroutine(LoadTableData(
			"EMPLOYEE_MASTER",
			row =>
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
						target.staffType = type;
					else
						Debug.LogWarning($"[EMPLOYEE] staffType 파싱 실패: {row["staffType"]}");

					Debug.Log($"[EMPLOYEE] 초기화 완료: {target.displayName}");
				}
			}));
	}

	private IEnumerator LoadTableData(string tableName, Action<JsonData> applyAction)
	{
		const int limit = 100;
		BackendReturnObject bro = null;

		bool isDone = false;

		// 첫 요청
		Backend.GameData.Get(tableName, new Where(), limit, callback =>
		{
			bro = callback;
			isDone = true;
		});

		yield return new WaitUntil(() => isDone);

		if (!bro.IsSuccess())
		{
			Debug.LogError($"[{tableName}] 첫 요청 실패: {bro.GetMessage()}");
			yield break;
		}

		HandleRows(bro, applyAction);

		// 다음 페이지가 있을 경우 반복
		while (true)
		{
			if (!bro.HasFirstKey())
				break;

			var firstKey = bro.FirstKeystring();
			isDone = false;

			Backend.GameData.Get(tableName, new Where(), limit, firstKey, callback =>
			{
				bro = callback;
				isDone = true;
			});

			yield return new WaitUntil(() => isDone);

			if (!bro.IsSuccess())
			{
				Debug.LogError($"[{tableName}] 페이징 요청 실패: {bro.GetMessage()}");
				yield break;
			}

			HandleRows(bro, applyAction);
			yield return new WaitForSeconds(0.1f); // 과부하 방지
		}
	}

	private void HandleRows(BackendReturnObject bro, Action<JsonData> applyAction)
	{
		foreach (var rowObj in bro.FlattenRows())
		{
			if (rowObj is JsonData row)
			{
				applyAction(row);
			}
			else
			{
				Debug.LogWarning("[LoadTableData] row가 JsonData가 아님");
			}
		}
	}


}
