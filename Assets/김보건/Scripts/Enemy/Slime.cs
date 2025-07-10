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
        Debug.Log("������ ���");
        base.Die();

        var dungeonManager = FindAnyObjectByType<GameController_bo>().GetManager<DungeonManager>();
        var floorData = dungeonManager.Config.selectedFloorData;

        if (floorData.currentStage < 10)
        {
            floorData.NextStage();
            // ������ ���ȯ
            Object.FindFirstObjectByType<MonsterSpawner>().SpawnNextStage();
        }
        else
        {
            Debug.Log("�������� 1-10 Ŭ����!");
            // TODO: ���� UI �Ǵ� LV2 ��ȯ ����
        }
    }
}
