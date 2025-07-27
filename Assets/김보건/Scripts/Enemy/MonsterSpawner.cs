using System.Collections;
using System.Threading;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public GameObject monsterPrefab;
    public GameObject bossMonsterPrefab;
    //public Vector3 spawnPosition = new Vector3(-10, 10, 0);

    [SerializeField] private Vector2 spawnXRange = new Vector2(-13f, -7f);
    [SerializeField] private Vector2 spawnYRange = new Vector2(1f, 6f);
    public float descendDuration = 10f;

    private int monsterKillCount = 0;
    [SerializeField] private int maxKillsBeforeBoss = 10;

    private bool hasSpawned = false;

    [SerializeField] private int totalMonsters = 50;
    [SerializeField] private float spawnInterval = 0.2f;

    private int currentSpawned = 0;
    private bool isSpawningWave = false;

    private bool bossReady = false;    // 킬 달성 여부
    private bool bossSpawned = false;  // 보스 실제 스폰 여부

    //void Start()
    //{
    //    if (hasSpawned) return;

    //    hasSpawned = true;


    //    Vector3 spawnPos = new Vector3(-73, 10, 0);

    //    GameObject slime = Instantiate(monsterPrefab, spawnPosition, Quaternion.identity);
    //    slime.transform.position = spawnPosition;

    //    EnemyBase enemyBase = slime.GetComponent<EnemyBase>();
    //    StartCoroutine(MoveDown(enemyBase, slime.transform));
    //}

    IEnumerator MoveDown(EnemyBase enemyBase, Transform slimeTransform)
    {
        float elapsed = 0f;
        Vector3 start = slimeTransform.position;
        Vector3 end = new Vector3(start.x, -8f, start.z);

        while (elapsed < descendDuration)
        {
            if (enemyBase != null)
            {
                // basePosition만 갱신하고, 실제 위치는 Update()에서 처리
                Vector3 basePos = Vector3.Lerp(start, end, elapsed / descendDuration);
                enemyBase.basePosition = basePos;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (enemyBase != null)
            enemyBase.basePosition = end;
    }


    public void SpawnNextStage()
    {

        if (hasSpawned)
        {
            Debug.LogWarning("이미 스폰됨! 중복 방지");
            return;
        }
        hasSpawned = true;

        var floorData = FindAnyObjectByType<GameController>()?.GetManager<DungeonManager>()?.Config.selectedFloorData;

        if (floorData == null)
        {
            Debug.LogError("SelectedFloorData없음");
            return;
        }

        GameObject prefabToSpawn;
        Vector3 spawnPos;

        // 만약 마지막 스테이지면 보스 소환
        if (floorData.IsLastStage()) 
        {
            prefabToSpawn = bossMonsterPrefab;
            spawnPos = new Vector3(-10f, 6f, 0f);
            Debug.Log("보스 몬스터 스폰");
        }
        else
        {
            prefabToSpawn = monsterPrefab;
            spawnPos = new Vector3(
            Random.Range(spawnXRange.x, spawnXRange.y), // -13 ~ -7
            Random.Range(spawnYRange.x, spawnYRange.y), // 1 ~ 6
            0
            );
            Debug.Log("일반 몬스터 스폰");
        }

        GameObject slime = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);

        EnemyBase enemyBase = slime.GetComponent<EnemyBase>();
        //StartCoroutine(MoveDown(enemyBase, slime.transform));
    }

    public void ResetSpawnFlag()
    {
        hasSpawned = false;
    }

    public void MonsterKilled()
    {
        monsterKillCount++;

        if (monsterKillCount >= totalMonsters)
        {
            if (monsterKillCount >= totalMonsters && !bossReady)
            {
                bossReady = true;     
                isSpawningWave = false;
                hasSpawned = false;  
                var floorData = FindAnyObjectByType<GameController>()?.GetManager<DungeonManager>()?.Config.selectedFloorData;
                floorData?.SetLastStage();

                EventBus<StageChangedEvent>.Raise(new StageChangedEvent(floorData.currentStage, floorData.IsLastStage()));
            }
        }

    }

    public void StartMonsterWave()
    {
        if (isSpawningWave || hasSpawned) return;
        currentSpawned = 0;
        monsterKillCount = 0;
        isSpawningWave = true;
        bossReady = false;      
        bossSpawned = false;
        StartCoroutine(SpawnMonsterWave());
    }

    private IEnumerator SpawnMonsterWave()
    {
        var floorData = FindAnyObjectByType<GameController>()?.GetManager<DungeonManager>()?.Config.selectedFloorData;

        while (currentSpawned < totalMonsters)
        {
            Vector3 spawnPos = new Vector3(
                Random.Range(spawnXRange.x, spawnXRange.y),
                Random.Range(spawnYRange.y, spawnYRange.x),
                0f);

            GameObject slime = Instantiate(monsterPrefab, spawnPos, Quaternion.identity);
            EnemyBase enemyBase = slime.GetComponent<EnemyBase>();
            //StartCoroutine(MoveDown(enemyBase, slime.transform));

            currentSpawned++;
            yield return new WaitForSeconds(spawnInterval);
        }

        // 몬스터 전부 소환 완료 → 기다림
        Debug.Log("일반 몬스터 50마리 모두 소환 완료");
    }

    public bool AllMonstersKilled()
    {
        return monsterKillCount >= totalMonsters && !isSpawningWave;
    }

    public bool TrySpawnBossOnce()
    {
        if (!bossReady || bossSpawned) return false; //이미 스폰했으면 무시
        bossSpawned = true;
        //보스만스폰
        hasSpawned = false; // SpawnNextStage 통과
        SpawnNextStage();   // 내부에서 IsLastStage()에 의해 bossMonsterPrefab 1마리만 스폰
        return true;
    }
}
