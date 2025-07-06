using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "DungeonMapDatabase", menuName = "Dungeon/DungeonMapDatabase")]
public class DungeonMapDatabase : ScriptableObject
{
    [System.Serializable]
    public class FloorEntry
    {
        public int floorNumber;               // 층 번호
        public GameObject mapPrefab;          // 맵 프리팹
        public Sprite backgroundImage;        // 배경 이미지
        public AudioClip bgm;                 // 층 전용 BGM
        public GameObject[] possibleMonsters; // 출현 몬스터 종류
        public int minMonsterCount;
        public int maxMonsterCount;
    }

    public List<FloorEntry> floorList;

    public FloorEntry GetFloorData(int floorNumber)
    {
        return floorList.Find(f => f.floorNumber == floorNumber);
    }
}