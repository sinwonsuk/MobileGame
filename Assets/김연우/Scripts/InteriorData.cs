using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Interior Data")]
public class InteriorData : ScriptableObject
{
    public string interiorName;      // ex) “소파”
    public Sprite icon;              // UI용 아이콘
    public GameObject prefab;        // 설치될 프리팹

    [Header("Placement Settings")]
    public Vector3 placementPosition;   // SO에 지정해 둔 월드 좌표
    public Vector3 placementRotation;   // SO에 지정해 둔 회전 (Euler)

    [TextArea] public string description;
}
