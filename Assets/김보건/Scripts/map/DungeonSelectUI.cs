using UnityEngine;

public class DungeonSelectUI : MonoBehaviour
{
    [SerializeField] private SelectedFloorData selectedFloorData;
    public GameObject dungeonInGameUI;

    public void OnClickFloorButton(int floor)
    {
        dungeonInGameUI.SetActive(true);
        selectedFloorData.selectedFloor = floor;
        selectedFloorData.ResetStage();
        selectedFloorData.isDungeonMode = true;
        //UnityEngine.SceneManagement.SceneManager.LoadScene("BoTest");

        var controller = FindAnyObjectByType<GameController_bo>();
        controller.ActiveOffAll(); // �ٸ� �Ŵ��� ����
        controller.GetManager<DungeonManager>().Init();

        gameObject.SetActive(false);
    }
}