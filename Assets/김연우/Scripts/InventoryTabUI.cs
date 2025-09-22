using UnityEngine;
using UnityEngine.UI;

public class InventoryTabUI : MonoBehaviour
{
    public Button materialsTabButton;
    public GameObject materialsPanel;
    public Button interiorsTabButton;
    public GameObject interiorsPanel;
    public Button huntersTabButton;
    public GameObject huntersPanel;

    private void Start()
    {
        materialsTabButton.onClick.AddListener(() => {
            SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click, false);
            ShowPanel(0);
        });
        interiorsTabButton.onClick.AddListener(() => {
            SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click, false);
            TutorialManager.Instance?.TriggerEvent("TouchInteriorButton");
            ShowPanel(1);
        });
        huntersTabButton.onClick.AddListener(() => {
            SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click, false);
            ShowPanel(2);
            TutorialManager.Instance?.TriggerEvent("TouchStaffButton");
        });
        ShowPanel(0); // 시작은 materials 패널
    }

    // 0: 재료, 1: 인테리어, 2: 헌터(직원)
    private void ShowPanel(int index)
    {
        materialsPanel.SetActive(index == 0);
        interiorsPanel.SetActive(index == 1);
        huntersPanel.SetActive(index == 2);
    }
}
