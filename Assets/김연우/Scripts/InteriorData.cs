using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Interior Data")]
public class InteriorData : ScriptableObject
{
    public string indate;
    public string interiorName;      // ex) “소파”
    public Sprite icon;              // UI용 아이콘
    public GameObject prefab;        // 설치될 프리팹
    public int BaseSalary;
    [Header("Placement Settings")]
    public Vector3 placementPosition;   // SO에 지정해 둔 월드 좌표
    
    [TextArea] public string description;
    public List<InteriorSkin> skins = new();

    [System.Serializable]
    public class InteriorSkin
    {
        public Sprite icon;
    }
}
