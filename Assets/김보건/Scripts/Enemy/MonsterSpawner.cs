using System.Collections;
using System.Threading;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public GameObject monsterPrefab;
    public GameObject bossMonsterPrefab;
    public Vector3 spawnPosition = new Vector3(-73, 10, 0);
    public float descendDuration = 10f;

    private bool hasSpawned = false;

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
        var floorData = FindAnyObjectByType<GameController_bo>()?.GetManager<DungeonManager>()?.Config.selectedFloorData;

        if (floorData == null)
        {
            Debug.LogError("SelectedFloorData없음");
            return;
        }

        GameObject prefabToSpawn;

        // 만약 마지막 스테이지면 보스 소환
        if (floorData.IsLastStage()) 
        {
            prefabToSpawn = bossMonsterPrefab;
            Debug.Log("보스 몬스터 스폰");
        }
        else
        {
            prefabToSpawn = monsterPrefab;
            Debug.Log("일반 몬스터 스폰");
        }

        GameObject slime = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
        slime.transform.position = spawnPosition;

        EnemyBase enemyBase = slime.GetComponent<EnemyBase>();
        StartCoroutine(MoveDown(enemyBase, slime.transform));
    }
}
