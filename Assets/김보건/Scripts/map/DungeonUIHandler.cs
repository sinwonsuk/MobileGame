using Unity.VisualScripting;
using UnityEngine;

public class DungeonUIHandler : MonoBehaviour
{
    public GameObject dungeonSelectUI; // ���� UI ������Ʈ
    public GameObject dungeonInGameUI; // ���� �� ������Ʈ


    // ���� ���� UI�� Ȱ��ȭ
    void Start()
    {
        dungeonInGameUI.SetActive(false);
        dungeonSelectUI.SetActive(false);
    }


    public void OnButtonClick()
    {
        // ���� ���� UI�� Ȱ��ȭ
        if (dungeonSelectUI != null)
        {
            dungeonSelectUI.SetActive(true);
        }
    }
}
