using UnityEngine;

public class DungeonSelectUI : MonoBehaviour
{
    [SerializeField] private SelectedFloorData selectedFloorData;
    public GameObject dungeonInGameUI;
    public AutoNextToggleButton autoNextToggle;


    public void OnClickFloorButton(int floor)
    {
        dungeonInGameUI.SetActive(false);
        selectedFloorData.selectedFloor = floor;
        selectedFloorData.ResetStage();

        EventBus<StageChangedEvent>.Raise(new StageChangedEvent(1, false));

        // 선택한 층의 던전 활성화
        selectedFloorData.isDungeonMode = true;
        EventBus<DungeonSlideToggleEvent>.Raise(new DungeonSlideToggleEvent(true));
        //UnityEngine.SceneManagement.SceneManager.LoadScene("BoTest");

        selectedFloorData.autoNextFloor = autoNextToggle.GetIsOn();

        EventBus<ButtonisActiveHandler>.Raise(new ButtonisActiveHandler(false));

        var controller = FindAnyObjectByType<GameController>();
        //controller.ActiveOffAll(); // 다른 매니저 끄고
        controller.GetManager<DungeonManager>().Init();

        gameObject.SetActive(false);
    }
}