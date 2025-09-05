using UnityEngine;
using UnityEngine.UI;

public class ShopTabManager : MonoBehaviour
{
    public Button CharacterTabButton;
    public GameObject CharacterPanel;

    public Button HuntersTabButton;
    public GameObject HuntersPanel;

    public Button ForestTabButton;
    public GameObject ForestPanel;

    [Header("헌터 팝업 부모 (HunterStaffPopup)")]
    public GameObject hunterStaffPopup;

    private void Start()
    {
        CharacterTabButton.onClick.AddListener(() => ShowPanel(0));
        HuntersTabButton.onClick.AddListener(() => ShowPanel(1));
        ForestTabButton.onClick.AddListener(() => ShowPanel(2));

        ShowPanel(0); // 시작은 캐릭터 패널
    }

    // 0: 캐릭터강화, 1: 헌터강화, 2: 파견
    private void ShowPanel(int index)
    {
        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click, false);

        CharacterPanel.SetActive(index == 0);
        HuntersPanel.SetActive(index == 1);
        ForestPanel.SetActive(index == 2);

        if (hunterStaffPopup != null)
        {
            bool hunterActive = (index == 1);
            hunterStaffPopup.SetActive(hunterActive);

            // 헌터 탭에서 나갈 때는 팝업 내부도 전부 닫아줌
            if (!hunterActive)
            {
                foreach (Transform child in hunterStaffPopup.transform)
                {
                    child.gameObject.SetActive(false);
                }
            }
        }
    }
}
