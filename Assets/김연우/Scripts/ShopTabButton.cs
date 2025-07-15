using UnityEngine;
using UnityEngine.UI;

public class ShopTabButton : MonoBehaviour
{
    [SerializeField] private ShopTabManager manager;
    [SerializeField] private int tabIndex;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            manager.ShowPanel(tabIndex);
        });
    }
}
