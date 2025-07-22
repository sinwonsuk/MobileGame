
using UnityEngine;
using UnityEngine.UI;

public class InventoryTabUI : MonoBehaviour
{
    public Button materialsTabButton;
    public GameObject materialsPanel;
    public Button interiorsTabButton;
    public GameObject interiorsPanel;

    private void Start()
    {
        materialsTabButton.onClick.AddListener(() => ShowPanel(true));
        interiorsTabButton.onClick.AddListener(() => ShowPanel(false));
        ShowPanel(true);
    }

    private void ShowPanel(bool showMaterials)
    {
        materialsPanel.SetActive(showMaterials);
        interiorsPanel.SetActive(!showMaterials);
    }
}
