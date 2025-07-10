using UnityEngine;

[CreateAssetMenu(fileName = "SelectedFloorData", menuName = "Dungeon/SelectedFloorData")]
public class SelectedFloorData : ScriptableObject
{
    public int selectedFloor;

    public int currentStage = 1; 

    public bool isDungeonMode = false;

    public void NextStage()
    {
        currentStage++;
    }

    public void ResetStage()
    {
        currentStage = 1;
    }
}