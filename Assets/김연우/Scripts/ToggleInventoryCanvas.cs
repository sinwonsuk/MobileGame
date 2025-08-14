// ToggleInventoryCanvas.cs
using UnityEngine;

public class ToggleInventoryCanvas : MonoBehaviour
{
    private void Awake()
    {
        // 인벤토리 토글은 기존대로
        EventBus<ToggleInventoryEvent>.OnEvent += OnToggleInventory;
        EventBus<CloseInventoryEvent>.OnEvent += OnCloseRequested;
        // 샵이 열리면 무조건 꺼지도록
        EventBus<ToggleHunterShopEvent>.OnEvent += OnHunterShopOpened;
        EventBus<ToggleRestaurantShopEvent>.OnEvent += OnRestaurantShopOpened;
    }

    private void Start()
    {
        // 시작 시 둘 다 꺼진 상태
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        EventBus<ToggleInventoryEvent>.OnEvent -= OnToggleInventory;
        EventBus<CloseInventoryEvent>.OnEvent -= OnCloseRequested;
        EventBus<ToggleHunterShopEvent>.OnEvent -= OnHunterShopOpened;
        EventBus<ToggleRestaurantShopEvent>.OnEvent -= OnRestaurantShopOpened;
    }

    private void OnToggleInventory(ToggleInventoryEvent evt)
    {
        // 본인 상태 토글
        gameObject.SetActive(!gameObject.activeSelf);
    }
    private void OnCloseRequested(CloseInventoryEvent _)
    {
        if (gameObject.activeSelf)
            gameObject.SetActive(false);
    }
    private void OnHunterShopOpened(ToggleHunterShopEvent evt)
    {
        // 샵 이벤트가 들어오면 자신이 켜져 있든 꺼져 있든 무조건 끔
        if (gameObject.activeSelf)
            gameObject.SetActive(false);
    }
    private void OnRestaurantShopOpened(ToggleRestaurantShopEvent evt)
    {
        // 샵 이벤트가 들어오면 자신이 켜져 있든 꺼져 있든 무조건 끔
        if (gameObject.activeSelf)
            gameObject.SetActive(false);
    }
}
