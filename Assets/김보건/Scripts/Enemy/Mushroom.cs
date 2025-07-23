using UnityEngine;

public class Mushroom : EnemyBase
{
    protected override void Start()
    {
        base.Start();

        var dungeonManager = FindAnyObjectByType<GameController>().GetManager<DungeonManager>();
        var floorData = dungeonManager.Config.selectedFloorData;

        int stage = floorData.currentStage;

        maxHp = 100f + stage * 0f;
        currentHp = maxHp;

        GetComponent<SpriteRenderer>().color = Color.Lerp(Color.white, Color.red, stage / 10f);
    }

    protected override void Die()
    {
        if (isDead) return;
        Debug.Log("사망");
        base.Die();

        var dungeonManager = FindAnyObjectByType<GameController>().GetManager<DungeonManager>();
        var floorData = dungeonManager.Config.selectedFloorData;

        Debug.Log($"autoNextFloor 값 확인: {floorData.autoNextFloor}");

        if (floorData.currentStage < 3)
        {
            floorData.NextStage();
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

    public override void OnDeathAnimationEnd()
    {
        base.OnDeathAnimationEnd();
    }
}
