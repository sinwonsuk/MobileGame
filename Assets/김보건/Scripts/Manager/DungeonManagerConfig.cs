using UnityEngine;
using TMPro;

[CreateAssetMenu(menuName = "Config/DungeonManagerConfig")]
public class DungeonManagerConfig : BaseScriptableObject
{
    public SelectedFloorData selectedFloorData;
    public DungeonMapDatabase mapDatabase;
    public GameObject playerPrefab;
    public Transform mapParent;
    public TextMeshProUGUI floorTextUI;

    public DungeonManagerConfig()
    {
        type = typeof(DungeonManagerConfig); // GameController¿¡¼­ »ç¿ëµÊ
    }
}