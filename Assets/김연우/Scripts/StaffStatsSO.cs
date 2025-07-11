// == StaffStatsSO.cs ==
using UnityEngine;

[CreateAssetMenu(fileName = "StaffStats", menuName = "Clicker/Staff Stats")]
public class StaffStatsSO : ScriptableObject
{
    [Header("Identity")]
    public string employeeId;     // indate key
    public string displayName;    // ���� �̸�

    [Header("Runtime Level")]
    [Tooltip("���� ���� (��Ÿ�ӿ� Init �� 1�� �����˴ϴ�)")]
    public int level;

    [Header("Base Stats")]
    public Sprite portrait;
    public int baseSalary;
    public int attack_Power;      // ���� 1 ���� ���ݷ�
    public int attack_Speed;      // ���� 1 ���� �ʴ� �߻� Ƚ��

    [Header("Per-Level Growth")]
    [Tooltip("������ �� �߰��Ǵ� ���ݷ�")]
    public int attack_PowerPerLevel = 1;
    [Tooltip("������ �� �߰��Ǵ� ���ݼӵ�")]
    public int attack_SpeedPerLevel = 1;

    [Header("Other")]
    [TextArea] public string explain;
    public GameObject itemPrefab;
}
