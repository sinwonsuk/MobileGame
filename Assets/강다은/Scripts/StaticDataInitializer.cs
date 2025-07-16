using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using LitJson;
using System.Linq;

public class StaticDataInitializer : MonoBehaviour
{
	[Header("ScriptableObject Lists")]
	[SerializeField] private List<IngredientData> ingredientDataList;
	[SerializeField] private List<FoodData> foodDataList;


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
	}

	IEnumerator LoadTableData(string tableName, System.Action<LitJson.JsonData> applyAction)
	{
		string offset = "";
		const int limit = 100;
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


			if (rows.Count < limit)
			{
				isEnd = true;
			}
			else
			{
				offset = rows[rows.Count - 1]["inDate"].ToString();
			}
		}
	}

}
