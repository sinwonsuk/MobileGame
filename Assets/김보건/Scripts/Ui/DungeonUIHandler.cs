using Unity.VisualScripting;
using UnityEngine;

public class DungeonUIHandler : MonoBehaviour
{
    public GameObject dungeonSelectUI; // 던전 UI 오브젝트
    public GameObject dungeonInGameUI; // 던전 맵 오브젝트


    // 던전 선택 UI를 활성화
    void Start()
    {
        dungeonInGameUI.SetActive(false);
        dungeonSelectUI.SetActive(false);
    }


    public void OnButtonClick()
    {
        // 던전 선택 UI를 활성화
        if (dungeonSelectUI != null)
        {
            dungeonSelectUI.SetActive(true);
        }
    }
}
