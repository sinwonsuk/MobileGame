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

    private void Start()
    {
        CharacterTabButton.onClick.AddListener(() => ShowPanel(0));
        HuntersTabButton.onClick.AddListener(() => ShowPanel(1));
        ForestTabButton.onClick.AddListener(() => ShowPanel(2));
        ShowPanel(0); // 시작은 materials 패널
    }

    // 0: 캐릭터강화, 1: 헌터강화, 2: 파견
    private void ShowPanel(int index)
    {
        CharacterPanel.SetActive(index == 0);
        HuntersPanel.SetActive(index == 1);
        ForestPanel.SetActive(index == 2);
    }
}