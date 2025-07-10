using UnityEngine;

public class Slime : EnemyBase
{
    protected override void Start()
    {
        base.Start();

        var dungeonManager = FindAnyObjectByType<GameController_bo>().GetManager<DungeonManager>();
        var floorData = dungeonManager.Config.selectedFloorData;

        int stage = floorData.currentStage;

        maxHp = 100f + stage * 50f;
        currentHp = maxHp;

        GetComponent<SpriteRenderer>().color = Color.Lerp(Color.green, Color.red, stage / 10f);
    }

    protected override void Die()
    {
        Debug.Log("슬라임 사망");
        base.Die();

        var dungeonManager = FindAnyObjectByType<GameController_bo>().GetManager<DungeonManager>();
        var floorData = dungeonManager.Config.selectedFloorData;

        if (floorData.currentStage < 10)
        {
            floorData.NextStage();
            // 슬라임 재소환
            Object.FindFirstObjectByType<MonsterSpawner>().SpawnNextStage();
        }
        else
        {
            Debug.Log("스테이지 1-10 클리어!");
            // TODO: 보상 UI 또는 LV2 전환 로직
        }
    }
}
