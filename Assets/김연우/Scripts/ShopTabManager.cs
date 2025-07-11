using UnityEngine;

public class ShopTabManager : MonoBehaviour
{
    [Header("�� ���� UI")]
    [SerializeField] private GameObject[] shopPanels;

    /// <summary>
    /// index ��° �гθ� �Ѱ� �������� ��
    /// </summary>
    public void ShowPanel(int index)
    {
        for (int i = 0; i < shopPanels.Length; i++)
            shopPanels[i].SetActive(i == index);
    }
}
