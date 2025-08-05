using UnityEngine;

public class PlacementPointRegister : MonoBehaviour
{
    private void Start()
    {
        if (EmployeeManager.Instance != null)
        {
            EmployeeManager.Instance.RegisterPlacementPoint(transform);
        }
    }
}
