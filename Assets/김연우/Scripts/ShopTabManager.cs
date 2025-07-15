using UnityEngine;

public class ShopTabManager : MonoBehaviour
{
    [Header("각 상점 UI")]
    [SerializeField] private GameObject[] shopPanels;

    /// <summary>
    /// index 번째 패널만 켜고 나머지는 끔
    /// </summary>
    public void ShowPanel(int index)
    {
        for (int i = 0; i < shopPanels.Length; i++)
            shopPanels[i].SetActive(i == index);
    }
}
