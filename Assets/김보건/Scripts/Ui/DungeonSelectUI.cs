using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class DungeonSelectUI : MonoBehaviour
{
    [SerializeField] private SelectedFloorData selectedFloorData;
    public GameObject dungeonInGameUI;
    public AutoNextToggleButton autoNextToggle;


    public void OnClickFloorButton(int floor)
    {
        ButtonManager.buttonClick = ButtonClick.none;
        var controller = FindAnyObjectByType<GameController>();
        var dungeonManager = controller.GetManager<DungeonManager>();

        var sm = SoundManager.GetInstance();
        if (sm != null)
        {
            sm.SetLocation(location.Dungeon);
            LocationState.Current = location.Dungeon;
            EventBus<LocationChangedEvent>.Raise(new LocationChangedEvent(location.Dungeon));
        }

        // 던전이 이미 진행 중이라면 (중간 이동)
        if (selectedFloorData.isDungeonMode)
        {
            dungeonManager.ResetDungeonEnvironment(); // 아이템 반영 + 초기화
        }


        //dungeonInGameUI.SetActive(false);
        selectedFloorData.selectedFloor = floor;
        selectedFloorData.ResetStage();

        EventBus<StageChangedEvent>.Raise(new StageChangedEvent(1, false));

        // 선택한 층의 던전 활성화
        selectedFloorData.isDungeonMode = true;
        EventBus<DungeonSlideToggleEvent>.Raise(new DungeonSlideToggleEvent(true));

        selectedFloorData.autoNextFloor = autoNextToggle.GetIsOn();
        EventBus<ButtonisActiveHandler>.Raise(new ButtonisActiveHandler(false));
        SoundManager.GetInstance().All_Sfx_Stop();

        dungeonManager.Init();

        dungeonInGameUI.SetActive(true);
        gameObject.SetActive(false);

        ButtonManager.instance.AllExit();
    }
}