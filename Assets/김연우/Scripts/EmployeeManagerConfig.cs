using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Clicker/EmployeeManager")]
public class EmployeeManagerConfig : ScriptableObject
{
    [SerializeField] private List<StaffStatsSO> employeeList = new List<StaffStatsSO>();
    public List<StaffStatsSO> EmployeeList => employeeList;
}
