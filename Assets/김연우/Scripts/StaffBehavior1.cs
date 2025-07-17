// == StaffBehavior.cs ==
using UnityEngine;
using System.Collections;

public class StaffBehavior1 : MonoBehaviour
{
    StaffStatsSO data;
    Transform boss;

    [SerializeField]
    float detectRange = 10f;

    public void Init(StaffStatsSO stats)
    {
        data = stats;
        data.level = 1;                       // 최초 레벨 1

    }

    public void LevelUp1()
    {
        data.level++;
        Debug.Log($"{data.displayName} leveled to {data.level}");
    }
}
