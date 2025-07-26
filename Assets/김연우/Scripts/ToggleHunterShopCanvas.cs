using UnityEngine;

public class ToggleHunterShopCanvas : MonoBehaviour
{
    private void Awake()
    {
        // 샵 토글은 기존대로
        EventBus<ToggleHunterShopEvent>.OnEvent += OnToggleHunterShop;
        // 인벤토리가 열리면 무조건 꺼지도록
        EventBus<ToggleRestaurantShopEvent>.OnEvent += OnToggleRestaurantShop;
        EventBus<ToggleInventoryEvent>.OnEvent += OnInventoryOpened;
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
    }

    private void OnToggleHunterShop(ToggleHunterShopEvent evt)
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
    private void OnToggleRestaurantShop(ToggleRestaurantShopEvent evt)
    {
        if (gameObject.activeSelf)
            gameObject.SetActive(false);
    }
    private void OnInventoryOpened(ToggleInventoryEvent evt)
    {
        if (gameObject.activeSelf)
            gameObject.SetActive(false);
    }
}
