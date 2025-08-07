using UnityEngine;

[CreateAssetMenu(fileName = "SelectedFloorData", menuName = "Dungeon/SelectedFloorData")]
public class SelectedFloorData : ScriptableObject
{
    public int selectedFloor;

    public int currentStage = 1; 

    public bool isDungeonMode = false;

    public bool autoNextFloor = false;

    public bool isBossStage = false; // 보스 스테이지 여부

    public void NextStage()
    {
        currentStage++;
    }

    public void ResetStage()
    {
        currentStage = 1;
        isBossStage = false;        

        // UI 갱신용 이벤트 발송
        EventBus<StageChangedEvent>.Raise(
            new StageChangedEvent(currentStage, isBossStage)
        );
    }

    public bool IsLastStage()
    {
        return currentStage >= 2;
    }

    public void SetLastStage()
    {
        currentStage = 2; 
    }
}