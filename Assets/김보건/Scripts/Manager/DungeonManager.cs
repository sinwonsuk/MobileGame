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

        // ��Ÿ�ӿ� TextMeshProUGUI ã�Ƽ� �Ҵ�
        if (config.floorTextUI == null)
        {
            var textObj = GameObject.Find("FloorText"); // ���̾��Ű���� FloorText��� �̸��� ������Ʈ
            if (textObj != null)
            {
                config.floorTextUI = textObj.GetComponent<TextMeshProUGUI>();
            }
        }


        if(config.mapParent == null)
        {
            var mapParentObj = GameObject.Find("MapParent"); // ���̾��Ű���� MapParent��� �̸��� ������Ʈ
            if(mapParentObj != null)
            {
                config.mapParent = mapParentObj.transform;
            }
            else
            {
                Debug.LogError("MapParent ������Ʈ�� ã�� �� �����ϴ�.");
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
            Debug.LogError("�ش� �� ���� ����");
            return;
        }

        var map = Object.Instantiate(floorData.mapPrefab, config.mapParent);
        var spawn = map.transform.Find("PlayerSpawnPoint");
        Object.Instantiate(config.playerPrefab, spawn != null ? spawn.position : Vector3.zero, Quaternion.identity);

        // UI�� ���� �� ǥ��
        if (config.floorTextUI != null)
            config.floorTextUI.text = $"LV{floor}";
        else
            Debug.LogWarning("floorTextUI�� �������� �ʾҽ��ϴ�.");

        var camera = Camera.main;
        if (camera != null)
        {
            camera.transform.position = new Vector3(map.transform.position.x, map.transform.position.y, camera.transform.position.z);
        }
    }

    public override void ActiveOff() { }

    public override void Update() { }
}
