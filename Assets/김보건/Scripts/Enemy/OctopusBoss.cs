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
        Debug.Log("���� ���");
        base.Die();

        var dungeonManager = FindAnyObjectByType<GameController_bo>().GetManager<DungeonManager>();
        var floorData = dungeonManager.Config.selectedFloorData;

        Debug.Log($"[Slime] autoNextFloor �� Ȯ��: {floorData.autoNextFloor}");

        if (floorData.currentStage < 3)
        {
            floorData.NextStage();
            // ������ ���ȯ
            Object.FindFirstObjectByType<MonsterSpawner>().SpawnNextStage();
        }
        else
        {
            Debug.Log("�������� 1-10 Ŭ����!");

            if (floorData.autoNextFloor)
            {
                // ���� ������ �̵�
                floorData.selectedFloor++;
                floorData.ResetStage();

                dungeonManager.LoadMap();
            }
            else
            {
                // �ٽ� 1-1���� �ݺ�
                floorData.ResetStage();
            }

            Object.FindFirstObjectByType<MonsterSpawner>().SpawnNextStage();
        }
    }
}
