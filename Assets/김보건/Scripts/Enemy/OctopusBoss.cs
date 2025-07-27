using UnityEngine;

public class OctopusBoss : EnemyBase
{
    protected override void Start()
    {
        base.Start();

        var dungeonManager = FindAnyObjectByType<GameController>().GetManager<DungeonManager>();
        var floorData = dungeonManager.Config.selectedFloorData;

        int stage = floorData.currentStage;

        maxHp = 100f;
        currentHp = maxHp;
    }

    protected override void Die()
    {
        if (isDead) return;
        Debug.Log("보스 사망");
        base.Die();

        var dungeonManager = FindAnyObjectByType<GameController>().GetManager<DungeonManager>();
        var floorData = dungeonManager.Config.selectedFloorData;

        Debug.Log($"autoNextFloor 값 확인: {floorData.autoNextFloor}");

        if (floorData.autoNextFloor)
        {
            floorData.selectedFloor++;
            floorData.ResetStage();
            EventBus<StageChangedEvent>.Raise(new StageChangedEvent(floorData.currentStage, false));
            dungeonManager.LoadMap();
        }
        else
        {
            floorData.ResetStage();
            EventBus<StageChangedEvent>.Raise(new StageChangedEvent(floorData.currentStage, false));
            //Object.FindFirstObjectByType<MonsterSpawner>()?.SpawnNextStage();  // 다시 1부터 시작
            var spawner = Object.FindFirstObjectByType<MonsterSpawner>();
            spawner?.StartMonsterWave();
        }

        //if (floorData.currentStage < 3)
        //{
        //    floorData.NextStage();
        //    // 문어 재소환
        //    Object.FindFirstObjectByType<MonsterSpawner>().SpawnNextStage();
        //}
        //else
        //{
        //    Debug.Log("스테이지 1-10 클리어!");

        //    if (floorData.autoNextFloor)
        //    {
        //        // 다음 층으로 이동
        //        floorData.selectedFloor++;
        //        floorData.ResetStage();

        //        dungeonManager.LoadMap();
        //    }
        //    else
        //    {
        //        // 다시 1-1부터 반복
        //        floorData.ResetStage();
        //    }

        //    Object.FindFirstObjectByType<MonsterSpawner>().SpawnNextStage();
        //}
    }

    public override void OnDeathAnimationEnd()
    {
        base.OnDeathAnimationEnd(); 
    }
}
