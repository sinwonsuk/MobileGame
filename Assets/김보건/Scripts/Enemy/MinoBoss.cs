using UnityEngine;

public class MinoBoss : EnemyBase
{
    private int stage;

    protected override float GetMaxHp() => 740.0f;

    protected override void Start()
    {

        var dungeonManager = FindAnyObjectByType<GameController>().GetManager<DungeonManager>();
        var floorData = dungeonManager.Config.selectedFloorData;

        int stage = floorData.currentStage;

        base.Start();

    }

    protected override void Die()
    {
        if (isDead) return;
        Debug.Log("보스 사망");
        base.Die();

        var dungeonManager = FindAnyObjectByType<GameController>().GetManager<DungeonManager>();
        var floorData = dungeonManager.Config.selectedFloorData;

        Debug.Log($"[Slime] autoNextFloor 값 확인: {floorData.autoNextFloor}");

        if (floorData.autoNextFloor)
        {
            floorData.selectedFloor++;
            floorData.ResetStage();
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
    }

    public override void OnDeathAnimationEnd()
    {
        base.OnDeathAnimationEnd();
    }
}
