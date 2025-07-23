// ToggleShopCanvas.cs
using UnityEngine;

public class ToggleShopCanvas : MonoBehaviour
{
    private void Awake()
    {
        // 샵 토글은 기존대로
        EventBus<ToggleShopEvent>.OnEvent += OnToggleShop;
        // 인벤토리가 열리면 무조건 꺼지도록
        EventBus<ToggleInventoryEvent>.OnEvent += OnInventoryOpened;
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        EventBus<ToggleShopEvent>.OnEvent -= OnToggleShop;
        EventBus<ToggleInventoryEvent>.OnEvent -= OnInventoryOpened;
    }

    private void OnToggleShop(ToggleShopEvent evt)
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    private void OnInventoryOpened(ToggleInventoryEvent evt)
    {
        if (gameObject.activeSelf)
            gameObject.SetActive(false);
    }
}
