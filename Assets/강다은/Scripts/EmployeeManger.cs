using BackEnd;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EmployeeManager : MonoBehaviour, IAutoSavable
{
	public static EmployeeManager Instance { get; private set; }

	[Header("Config: 전체 직원 데이터")]
	public StaffStatsSO[] allEmployees;

	[Header("Config: 전체 런타임 직원 데이터")]
	public RuntimeStaffStatsSO[] allRunTimeEmployees;

	private bool employeeDataLoaded = false;

	private void Awake()
	{
		if (Instance == null)
			Instance = this;
		else
		{
			Destroy(gameObject);
			return;
		}

		DontDestroyOnLoad(gameObject);
		InitializeDisplayNamesFromStatic();
	}

	public IEnumerator InsertEmployeesIfNotExists(string ownerIndate)
	{
		string offset = "";
		bool isEnd = false;
		HashSet<string> existingIndates = new HashSet<string>();

		while (!isEnd)
		{
			bool isDone = false;
			BackendReturnObject bro = null;

			var where = new Where();
			where.Equal("owner_inDate", ownerIndate);

			Backend.GameData.Get("EMPLOYEE_PLAYER", where, 100, offset, callback =>
			{
				bro = callback;
				isDone = true;
			});

			yield return new WaitUntil(() => isDone);

			if (!bro.IsSuccess())
			{
				Debug.LogError("[InsertEmployeesIfNotExists] 조회 실패: " + bro.GetMessage());
				yield break;
			}

			var rows = bro.FlattenRows();
			foreach (var rowObj in rows)
			{
				var row = rowObj as LitJson.JsonData;
				if (row == null) continue;

				existingIndates.Add(row["employeeIndate"].ToString());
			}


			var json = LitJson.JsonMapper.ToObject(bro.GetReturnValue());
			offset = json.ContainsKey("offset") ? json["offset"].ToString() : null;
			isEnd = string.IsNullOrEmpty(offset);
		}

		foreach (var emp in allRunTimeEmployees)
		{
			if (!existingIndates.Contains(emp.indate))
			{
				Param param = new Param();
				param.Add("employeeIndate", emp.indate);
				param.Add("employeeCustomLevel", "0");
				param.Add("employeeName", emp.displayName);

				bool done = false;
				BackendReturnObject insertBro = null;

				Backend.GameData.Insert("EMPLOYEE_PLAYER", param, callback =>
				{
					insertBro = callback;
					done = true;
				});

				yield return new WaitUntil(() => done);

				if (insertBro.IsSuccess())
				{
					Debug.Log($"[직원 Insert 성공] {emp.indate}");
					emp.level = 0;
					emp.isDirty = false;
				}
				else
				{
					Debug.LogError($"[직원 Insert 실패] {emp.indate} : {insertBro.GetMessage()}");
				}
			}
		}
	}

	public IEnumerator LoadEmployeeData(string ownerIndate)
	{
		string offset = "";
		bool isEnd = false;

		while (!isEnd)
		{
			bool isDone = false;
			BackendReturnObject bro = null;

			var where = new Where();
			where.Equal("owner_inDate", ownerIndate);

			Backend.GameData.Get("EMPLOYEE_PLAYER", where, 100, offset, callback =>
			{
				bro = callback;
				isDone = true;
			});

			yield return new WaitUntil(() => isDone);

			if (!bro.IsSuccess())
			{
				Debug.LogError("[LoadEmployeeData] 실패: " + bro.GetMessage());
				yield break;
			}

			var rows = bro.FlattenRows();
			foreach (var rowObj in rows)
			{
				var row = rowObj as LitJson.JsonData;
				if (row == null) continue;

				string empIndate = row["employeeIndate"].ToString();
				int level = int.Parse(row["employeeCustomLevel"].ToString());

				var emp = allRunTimeEmployees.FirstOrDefault(e => e.indate == empIndate);
				if (emp != null)
				{
					emp.level = level;
					emp.isDirty = false;
				}
			}

			var json = LitJson.JsonMapper.ToObject(bro.GetReturnValue());
			offset = json.ContainsKey("offset") ? json["offset"].ToString() : null;
			isEnd = string.IsNullOrEmpty(offset);
		}

		employeeDataLoaded = true;

		AutoSaveManager.Instance?.RegisterAutoSavable(this);
	}

	public void AutoSave()
	{
		if (!employeeDataLoaded)
		{
			Debug.LogWarning("[AutoSave 차단] 직원 데이터 로딩 안 됨");
			return;
		}

		SaveEmployeeData();
	}

	public void SaveEmployeeData()
	{
		string ownerIndate = Backend.UserInDate;

		foreach (var emp in allRunTimeEmployees)
		{
			if (!emp.isDirty) continue;

			Where where = new Where();
			where.Equal("owner_inDate", ownerIndate);
			where.Equal("employeeIndate", emp.indate);

			Param param = new Param();
			param.Add("employeeCustomLevel", emp.level);
			param.Add("employeeName", emp.displayName);

			Backend.GameData.Update("EMPLOYEE_PLAYER", where, param);
			emp.isDirty = false;
		}

		Debug.Log("[EmployeeManager] 변경된 직원 데이터 저장 완료");
	}

	private void InitializeDisplayNamesFromStatic()
	{
		foreach (var runtime in allRunTimeEmployees)
		{
			var staticData = allEmployees.FirstOrDefault(s => s.indate == runtime.indate);
			if (staticData != null)
			{
				runtime.displayName = staticData.displayName;
			}
			else
			{
				Debug.LogWarning($"[초기화 실패] {runtime.indate} 에 해당하는 마스터 직원 데이터가 없습니다.");
			}
		}
	}

}
