using UnityEngine;

[CreateAssetMenu(fileName = "SelectedFloorData", menuName = "Dungeon/SelectedFloorData")]
public class SelectedFloorData : ScriptableObject
{
    public int selectedFloor;

    public bool isDungeonMode = false;
}