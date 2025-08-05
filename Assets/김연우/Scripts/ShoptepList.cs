using UnityEngine;
using UnityEngine.UI;
public class ShoptepList : MonoBehaviour
{
    public Button RestTabButton;
    public GameObject RestPanel;
    public Button InteriorTabButton;
    public GameObject InteriorPanel;


    private void Start()
    {
        RestTabButton.onClick.AddListener(() => ShowPanel(0));
        InteriorTabButton.onClick.AddListener(() => ShowPanel(1));
        ShowPanel(0); // 시작은 materials 패널
    }

    // 0: 레스토랑 직원 강화, 1: 인테리어, 
    private void ShowPanel(int index)
    {
        RestPanel.SetActive(index == 0);
        InteriorPanel.SetActive(index == 1);
    }

}