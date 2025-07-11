using UnityEngine;
using TMPro;

public class DungeonManager : baseManager
{
    private DungeonManagerConfig config;
    public DungeonManagerConfig Config => config;

    private GameObject currentMapInstance;

    private Vector3 originalCameraPos;
    private bool hasSavedCameraPos = false;

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

    private void OnEnable()
    {
        EventBus<AutoNextFloorChangedEvent>.OnEvent += OnAutoNextChanged;
    }

    private void OnDisable()
    {
        EventBus<AutoNextFloorChangedEvent>.OnEvent -= OnAutoNextChanged;
    }

    void OnAutoNextChanged(AutoNextFloorChangedEvent evt)
    {
        var selectedFloorData = this.Config.selectedFloorData;
        selectedFloorData.autoNextFloor = evt.isAutoNext;

        Debug.Log($"[DungeonManager] autoNextFloor ������: {evt.isAutoNext}");
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

        map.GetComponentInChildren<MonsterSpawner>()?.SpawnNextStage();

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
            if (originalCameraPos == Vector3.zero)
            {
                originalCameraPos = camera.transform.position;
                hasSavedCameraPos = true;
            }

            camera.transform.position = new Vector3(map.transform.position.x, map.transform.position.y, camera.transform.position.z);
        }
    }

    public void LoadMap()
    {
        int floor = Config.selectedFloorData.selectedFloor;
        Debug.Log($"[DungeonManager] {floor}���� ���� �ҷ��ɴϴ�.");

        // ���� �� ����
        foreach (Transform child in Config.mapParent)
        {
            Object.Destroy(child.gameObject);
        }

        // �� �� �ν��Ͻ�ȭ
        var newMapPrefab = Config.mapDatabase.GetMapPrefab(floor);
        if (newMapPrefab != null)
            Object.Instantiate(newMapPrefab, Config.mapParent);
    }

    public void ExitDungeon()
    {
        Debug.Log("[DungeonManager] ���� ���� ó��");

        // �� ����
        if (config.mapParent != null)
        {
            foreach (Transform child in config.mapParent)
            {
                Object.Destroy(child.gameObject);
            }
        }

        //ī�޶� ��ġ ����
        var camera = Camera.main;
        if (camera != null)
            camera.transform.position = originalCameraPos;


        config.selectedFloorData.isDungeonMode = false;
        hasSavedCameraPos = false;
    }

    public override void ActiveOff() { }

    public override void Update() { }
}
