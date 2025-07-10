using UnityEngine;

public class OctopusBoss : EnemyBase
{
    protected override void Start()
    {
        base.Start();

        var dungeonManager = FindAnyObjectByType<GameController_bo>().GetManager<DungeonManager>();
        var floorData = dungeonManager.Config.selectedFloorData;

        int stage = floorData.currentStage;

        maxHp = 500f;
        currentHp = maxHp;
    }

    protected override void Die()
    {
        Debug.Log("보스 사망");
        base.Die();

        var dungeonManager = FindAnyObjectByType<GameController_bo>().GetManager<DungeonManager>();
        var floorData = dungeonManager.Config.selectedFloorData;

        Debug.Log($"[Slime] autoNextFloor 값 확인: {floorData.autoNextFloor}");

        if (floorData.currentStage < 3)
        {
            floorData.NextStage();
            // 슬라임 재소환
            Object.FindFirstObjectByType<MonsterSpawner>().SpawnNextStage();
        }
        else
        {
            Debug.Log("스테이지 1-10 클리어!");

            if (floorData.autoNextFloor)
            {
                // 다음 층으로 이동
                floorData.selectedFloor++;
                floorData.ResetStage();

                dungeonManager.LoadMap();
            }
            else
            {
                // 다시 1-1부터 반복
                floorData.ResetStage();
            }

            Object.FindFirstObjectByType<MonsterSpawner>().SpawnNextStage();
        }
    }
}
