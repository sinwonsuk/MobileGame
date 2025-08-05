using System;

[Serializable]
public class EmployeeSlot
{
    public StaffStatsSO staffData;
    public RuntimeStaffStatsSO runtimeData;

    public EmployeeSlot(StaffStatsSO data, RuntimeStaffStatsSO runtimeData)
    {
        staffData = data;
        this.runtimeData = runtimeData;
    }

    public bool IsAssigned => runtimeData.isAssigned;
    public bool IsOwned => runtimeData.isOwned;
    public int AssignedIndex => runtimeData.assignedIndex;
}
