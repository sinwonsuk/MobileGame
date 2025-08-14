using UnityEngine;

public class ToggleRestaurantShopCanvas : MonoBehaviour
{
    private void Awake()
    {
        EventBus<ToggleRestaurantShopEvent>.OnEvent += OnToggleRestaurantShop;
        // 샵 토글은 기존대로
        EventBus<ToggleHunterShopEvent>.OnEvent += OnToggleHunterShop;
        // 인벤토리가 열리면 무조건 꺼지도록
        EventBus<ToggleInventoryEvent>.OnEvent += OnInventoryOpened;
        EventBus<CloseRestaurantShopEvent>.OnEvent += OnCloseRequested;
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        EventBus<ToggleHunterShopEvent>.OnEvent -= OnToggleHunterShop;
        EventBus<ToggleRestaurantShopEvent>.OnEvent -= OnToggleRestaurantShop;
        // 인벤토리가 열리면 무조건 꺼지도록
        EventBus<ToggleInventoryEvent>.OnEvent -= OnInventoryOpened;
        EventBus<CloseRestaurantShopEvent>.OnEvent -= OnCloseRequested;
    }
    private void OnToggleRestaurantShop(ToggleRestaurantShopEvent evt)
    {
        gameObject.SetActive(!gameObject.activeSelf);

    }
    private void OnToggleHunterShop(ToggleHunterShopEvent evt)
    {
        if (gameObject.activeSelf)
            gameObject.SetActive(false);
    }
    
    private void OnInventoryOpened(ToggleInventoryEvent evt)
    {
        if (gameObject.activeSelf)
            gameObject.SetActive(false);
    }
    private void OnCloseRequested(CloseRestaurantShopEvent _)
    {
        if (gameObject.activeSelf)
            gameObject.SetActive(false);
    }
}
