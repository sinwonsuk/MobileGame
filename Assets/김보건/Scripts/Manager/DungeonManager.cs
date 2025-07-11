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

        Debug.Log($"[DungeonManager] autoNextFloor 설정됨: {evt.isAutoNext}");
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

        map.GetComponentInChildren<MonsterSpawner>()?.SpawnNextStage();

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
        Debug.Log($"[DungeonManager] {floor}층의 맵을 불러옵니다.");

        // 기존 맵 제거
        foreach (Transform child in Config.mapParent)
        {
            Object.Destroy(child.gameObject);
        }

        // 새 맵 인스턴스화
        var newMapPrefab = Config.mapDatabase.GetMapPrefab(floor);
        if (newMapPrefab != null)
            Object.Instantiate(newMapPrefab, Config.mapParent);
    }

    public void ExitDungeon()
    {
        Debug.Log("[DungeonManager] 던전 종료 처리");

        // 맵 제거
        if (config.mapParent != null)
        {
            foreach (Transform child in config.mapParent)
            {
                Object.Destroy(child.gameObject);
            }
        }

        //카메라 위치 복귀
        var camera = Camera.main;
        if (camera != null)
            camera.transform.position = originalCameraPos;


        config.selectedFloorData.isDungeonMode = false;
        hasSavedCameraPos = false;
    }

    public override void ActiveOff() { }

    public override void Update() { }
}
