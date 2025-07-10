using UnityEngine;

public class DungeonSelectUI : MonoBehaviour
{
    [SerializeField] private SelectedFloorData selectedFloorData;
    public GameObject dungeonInGameUI;
    public AutoNextToggleButton autoNextToggle;


    public void OnClickFloorButton(int floor)
    {
        dungeonInGameUI.SetActive(true);
        selectedFloorData.selectedFloor = floor;
        selectedFloorData.ResetStage();
        selectedFloorData.isDungeonMode = true;
        //UnityEngine.SceneManagement.SceneManager.LoadScene("BoTest");

        selectedFloorData.autoNextFloor = autoNextToggle.GetIsOn();


        var controller = FindAnyObjectByType<GameController_bo>();
        controller.ActiveOffAll(); // 다른 매니저 끄고
        controller.GetManager<DungeonManager>().Init();

        gameObject.SetActive(false);
    }
}