using UnityEngine;

public class Mimic : EnemyBase
{
    private int stage;

    protected override float GetMaxHp() => 400.0f;

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
