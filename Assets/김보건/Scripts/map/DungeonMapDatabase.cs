using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "DungeonMapDatabase", menuName = "Dungeon/DungeonMapDatabase")]
public class DungeonMapDatabase : ScriptableObject
{
    [System.Serializable]
    public class FloorEntry
    {
        public int floorNumber;               // �� ��ȣ
        public GameObject mapPrefab;          // �� ������
        public Sprite backgroundImage;        // ��� �̹���
        public AudioClip bgm;                 // �� ���� BGM
        public GameObject[] possibleMonsters; // ���� ���� ����
        public int minMonsterCount;
        public int maxMonsterCount;
    }

    public List<FloorEntry> floorList;

    public FloorEntry GetFloorData(int floorNumber)
    {
        return floorList.Find(f => f.floorNumber == floorNumber);
    }

    public GameObject GetMapPrefab(int floorNumber)
    {
        foreach (var floorData in floorList)
        {
            if (floorData.floorNumber == floorNumber)
                return floorData.mapPrefab;
        }

        Debug.LogWarning($"[DungeonMapDatabase] Floor {floorNumber}�� �ش��ϴ� �� �������� ã�� �� �����ϴ�.");
        return null;
    }
}