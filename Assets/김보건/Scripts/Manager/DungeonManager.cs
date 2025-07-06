using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    [SerializeField] private SelectedFloorData selectedFloorData;
    [SerializeField] private DungeonMapDatabase mapDatabase;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform mapParent;

    void Start()
    {
        int floor = selectedFloorData.selectedFloor;
        var floorData = mapDatabase.GetFloorData(floor);

        if (floorData == null)
        {
            Debug.LogError("�ش� �� ���� ����");
            return;
        }

        var map = Instantiate(floorData.mapPrefab, mapParent);
        var spawn = map.transform.Find("PlayerSpawnPoint");
        Instantiate(playerPrefab, spawn != null ? spawn.position : Vector3.zero, Quaternion.identity);
    }
}
