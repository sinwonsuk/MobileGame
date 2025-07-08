using BackEnd;
using LitJson;
using UnityEngine;

/* 사용법
 * StaticDataUploader.InsertStaticData("테이블명", data);
 * 
 * data는 Param 형태로, 예시:
 * food.Add("foodName", "버섯스프");
 * food.Add("foodImagePath", "/images/foods/mushroom.png");
 * food.Add("foodPrice", 150);
 */

/*
 * 주의사항
 * '정적 테이블 구조'의 데이터만 삽입할 것.
 *  이 함수를 사용 가능한 테이블은 아래와 같음.
 *  FOODS, EMPLOYEES_MASTER, FURNITURE, FOOD_INGREDIENTS, INGREDITENS
 *  관리자 계정에서만 사용 할 것. (관리자 indate 값으로 맞춰주기 위함)
 *  관리자 계정 user1 / 1234
 *  FunctionTest.cs에서 사용 예시 있음.
 */


public static class StaticDataUploader
{
	// 하나씩만 삽입
	public static void InsertStaticData(string tableName, Param data)
	{
		Backend.GameData.Insert(tableName, data, callback =>
		{
			if (callback.IsSuccess())
			{
				Debug.Log($"[성공] {tableName} 테이블에 데이터 삽입 완료");
				Debug.Log($"[삽입된 데이터] {ParamToJson(data)}");
			}
			else
			{
				Debug.LogError($"[실패] {tableName} 삽입 실패: {callback.GetMessage()}");
			}
		});
	}

	private static string ParamToJson(Param param)
	{
		return LitJson.JsonMapper.ToJson(param.GetValue());
	}
}
