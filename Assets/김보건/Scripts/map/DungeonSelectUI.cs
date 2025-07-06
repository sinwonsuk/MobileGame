using UnityEngine;

public class DungeonSelectUI : MonoBehaviour
{
    [SerializeField] private SelectedFloorData selectedFloorData;

    public void OnClickFloorButton(int floor)
    {
        selectedFloorData.selectedFloor = floor;
        UnityEngine.SceneManagement.SceneManager.LoadScene("BoTest");
    }
}