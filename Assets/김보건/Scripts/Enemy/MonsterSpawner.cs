using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public GameObject monsterPrefab;
    public float spawnInterval = 3f;
    public int spawnCount = 1;

    [Header("���� ����")]
    public Vector2 spawnAreaMin = new Vector2(-5, -3);  // ���ϴ�
    public Vector2 spawnAreaMax = new Vector2(5, 3);    // ����

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnMonsters();
            timer = 0f;
        }
    }

    void SpawnMonsters()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            Vector2 randomPos = new Vector2(
                Random.Range(spawnAreaMin.x, spawnAreaMax.x),
                Random.Range(spawnAreaMin.y, spawnAreaMax.y)
            );

            Instantiate(monsterPrefab, randomPos, Quaternion.identity);
        }
    }

}
