using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class DungeonManager : baseManager
{
    private DungeonManagerConfig config;
    public DungeonManagerConfig Config => config;

    private GameObject currentMapInstance;

    private Vector3 originalCameraPos;
    private bool hasSavedCameraPos = false;

    // 드랍아이템 저장용 딕셔너리
    private readonly Dictionary<string, int> tempLoot = new();

    public DungeonManager(DungeonManagerConfig config)
    {
        this.config = config;

        if (config.selectedFloorData != null)
            config.selectedFloorData.isDungeonMode = false;

        // 런타임에 TextMeshProUGUI 찾아서 할당
        if (config.floorTextUI == null)
        {
            var textObj = GameObject.Find("FloorText"); 
            if (textObj != null)
            {
                config.floorTextUI = textObj.GetComponent<TextMeshProUGUI>();
            }
        }


        if(config.mapParent == null)
        {
            var mapParentObj = GameObject.Find("MapParent");
            if(mapParentObj != null)
            {
                config.mapParent = mapParentObj.transform;
            }
            else
            {
                Debug.LogError("MapParent 오브젝트를 찾을 수 없음");
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

        Debug.Log($"던전매니저autoNextFloor 설정됨: {evt.isAutoNext}");
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
            Debug.LogWarning("floorTextUI가 없음");

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
        Debug.Log($"[DungeonManager] {floor}층의 맵 가져옴");

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
        Debug.Log("던전 종료 처리");
        CommitLootToInventory();
        // 맵 제거
        if (config.mapParent != null)
        {
            foreach (Transform child in config.mapParent)
            {
                Object.Destroy(child.gameObject);
            }
        }

        //미사일제거
        foreach (var bullet in Object.FindObjectsByType<BaseBullet>(FindObjectsSortMode.None))
        {
            GameObject.Destroy(bullet.gameObject);
        }

        //플레이어제거
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            GameObject.Destroy(player);
        }

        //몬스터제거
        foreach (var monster in Object.FindObjectsByType<EnemyBase>(FindObjectsSortMode.None))
            GameObject.Destroy(monster.gameObject);

        //드랍 아이템 제거
        foreach (var drop in Object.FindObjectsByType<DroppableItem>(FindObjectsSortMode.None))
            GameObject.Destroy(drop.gameObject);

        //카메라 위치 복귀
        var camera = Camera.main;
        if (camera != null)
            camera.transform.position = originalCameraPos;


        config.selectedFloorData.isDungeonMode = false;
        hasSavedCameraPos = false;
    }

    // 아이템 임시 저장
    public void AddTempItem(string name, int qty = 1)
    {
        if (string.IsNullOrEmpty(name)) return;

        if (tempLoot.ContainsKey(name))
            tempLoot[name] += qty;
        else
            tempLoot[name] = qty;

        Debug.Log($"[DungeonLoot] {name} +{qty} (누적 {tempLoot[name]})");
    }

    // 인벤토리에 한번에 반영
    private void CommitLootToInventory()
    {
        foreach (var kvp in tempLoot)
            InventoryManager.Instance.AddItem(kvp.Key, kvp.Value);

        tempLoot.Clear();   // 반영이 끝났으니 비워 두기
    }

    public override void ActiveOff() { }

    public override void Update() { }
}
