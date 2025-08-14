using UnityEngine;

public class WoodGolemBoss : EnemyBase
{
    protected override void Start()
    {
        base.Start();

        var dungeonManager = FindAnyObjectByType<GameController>().GetManager<DungeonManager>();
        var floorData = dungeonManager.Config.selectedFloorData;

        int stage = floorData.currentStage;

        maxHp = 190f;
        currentHp = maxHp;
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
