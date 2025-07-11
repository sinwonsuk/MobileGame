using UnityEngine;

public class octopus : EnemyBase
{
    protected override void Start()
    {
        base.Start();

        var dungeonManager = FindAnyObjectByType<GameController>().GetManager<DungeonManager>();
        var floorData = dungeonManager.Config.selectedFloorData;

        int stage = floorData.currentStage;

        maxHp = 100f + stage * 0f;
        currentHp = maxHp;

        GetComponent<SpriteRenderer>().color = Color.Lerp(Color.green, Color.red, stage / 10f);
    }

    protected override void Die()
    {
        Debug.Log("���� ���");
        base.Die();

        var dungeonManager = FindAnyObjectByType<GameController>().GetManager<DungeonManager>();
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

    //private IEnumerator DelayNextFloor(DungeonManager dungeonManager, SelectedFloorData floorData)
    //{
    //    yield return new WaitForSeconds(3f);

    //    floorData.selectedFloor++;
    //    floorData.ResetStage();
    //    dungeonManager.LoadMap();

    //}
}
