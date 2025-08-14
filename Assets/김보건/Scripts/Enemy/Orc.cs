using UnityEngine;

public class Orc : EnemyBase
{
    protected override void Start()
    {
        base.Start();

        var dungeonManager = FindAnyObjectByType<GameController>().GetManager<DungeonManager>();
        var floorData = dungeonManager.Config.selectedFloorData;

        int stage = floorData.currentStage;

        maxHp = 53f;
        currentHp = maxHp;

        GetComponent<SpriteRenderer>().color = Color.Lerp(Color.white, Color.red, stage / 10f);
    }

    protected override void Die()
    {
        if (isDead) return;
        Debug.Log("»ç¸Á");
        base.Die();
        var spawner = Object.FindFirstObjectByType<MonsterSpawner>();
        //spawner?.MonsterKilled();

        spawner?.TrySpawnBossOnce();
    }

    public override void OnDeathAnimationEnd()
    {
        base.OnDeathAnimationEnd();
    }
}
