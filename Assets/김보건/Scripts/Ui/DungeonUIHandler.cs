using Unity.VisualScripting;
using UnityEngine;

public class DungeonUIHandler : MonoBehaviour
{
    public GameObject dungeonSelectUI; // 던전 UI 오브젝트

    // 던전 선택 UI를 활성화
    void Start()
    {
        dungeonSelectUI.SetActive(false);
    }


    public void OnButtonClick()
    {
        if (dungeonSelectUI != null)
        {
            bool currentState = dungeonSelectUI.activeSelf;
            dungeonSelectUI.SetActive(!currentState);
        }
    }
}
