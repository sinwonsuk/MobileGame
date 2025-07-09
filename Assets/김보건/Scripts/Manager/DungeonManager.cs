using UnityEngine;
using TMPro;

public class DungeonManager : baseManager
{
    private DungeonManagerConfig config;

    public DungeonManager(DungeonManagerConfig config)
    {
        this.config = config;

        if (config.selectedFloorData != null)
            config.selectedFloorData.isDungeonMode = false;

        // 런타임에 TextMeshProUGUI 찾아서 할당
        if (config.floorTextUI == null)
        {
            var textObj = GameObject.Find("FloorText"); // 하이어라키에서 FloorText라는 이름의 오브젝트
            if (textObj != null)
            {
                config.floorTextUI = textObj.GetComponent<TextMeshProUGUI>();
            }
        }


        if(config.mapParent == null)
        {
            var mapParentObj = GameObject.Find("MapParent"); // 하이어라키에서 MapParent라는 이름의 오브젝트
            if(mapParentObj != null)
            {
                config.mapParent = mapParentObj.transform;
            }
            else
            {
                Debug.LogError("MapParent 오브젝트를 찾을 수 없습니다.");
            }
        }
    }

    public override void Init()
    {
        if (config.selectedFloorData == null || config.selectedFloorData.isDungeonMode == false)
            return;

        int floor = config.selectedFloorData.selectedFloor;
        var floorData = config.mapDatabase.GetFloorData(floor);

        if (floorData == null)
        {
            Debug.LogError("해당 층 정보 없음");
            return;
        }

        var map = Object.Instantiate(floorData.mapPrefab, config.mapParent);
        var spawn = map.transform.Find("PlayerSpawnPoint");
        Object.Instantiate(config.playerPrefab, spawn != null ? spawn.position : Vector3.zero, Quaternion.identity);

        // UI에 현재 층 표시
        if (config.floorTextUI != null)
            config.floorTextUI.text = $"LV{floor}";
        else
            Debug.LogWarning("floorTextUI가 설정되지 않았습니다.");

        var camera = Camera.main;
        if (camera != null)
        {
            camera.transform.position = new Vector3(map.transform.position.x, map.transform.position.y, camera.transform.position.z);
        }
    }

    public override void ActiveOff() { }

    public override void Update() { }
}
