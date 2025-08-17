using UnityEngine;

public class ToggleRestaurantShopCanvas : MonoBehaviour
{
    void Awake()
    {
        EventBus<ToggleRestaurantShopEvent>.OnEvent += OnToggle;
        EventBus<CloseRestaurantShopEvent>.OnEvent += OnCloseRequested;

        // ¡Ú ÇåÅÍ ¼¥ÀÌ ÄÑÁö¸é ³ª´Â ´Ý±â (ÇåÅÍ¿ÍÀÇ Áßº¹ ¹æÁö)
        EventBus<ToggleHunterShopEvent>.OnEvent += OnHunterOpened;
        EventBus<ToggleInventoryEvent>.OnEvent += OnInventoryOpened;
    }

    void OnDestroy()
    {
        EventBus<ToggleRestaurantShopEvent>.OnEvent -= OnToggle;
        EventBus<CloseRestaurantShopEvent>.OnEvent -= OnCloseRequested;

        EventBus<ToggleHunterShopEvent>.OnEvent -= OnHunterOpened;
        EventBus<ToggleInventoryEvent>.OnEvent -= OnInventoryOpened;
    }

    void Start() => gameObject.SetActive(false);

    void OnToggle(ToggleRestaurantShopEvent _)
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    void OnCloseRequested(CloseRestaurantShopEvent _)
    {
        if (!gameObject.activeSelf) return;

        gameObject.SetActive(false);
        if (ButtonManager.buttonClick == ButtonClick.Restaurant)
            ButtonManager.buttonClick = ButtonClick.none;
    }

    // ¡Ú ÇåÅÍ ¼¥ ¿­¸² ¡æ ·¹½ºÅä ´Ý±â
    void OnHunterOpened(ToggleHunterShopEvent _)
    {
        if (!gameObject.activeSelf) return;

        gameObject.SetActive(false);
        if (ButtonManager.buttonClick == ButtonClick.Restaurant)
            ButtonManager.buttonClick = ButtonClick.none;
    }
    void OnInventoryOpened(ToggleInventoryEvent _)
    {
        if (!gameObject.activeSelf) return;
        gameObject.SetActive(false);
        if (ButtonManager.buttonClick == ButtonClick.Restaurant)
            ButtonManager.buttonClick = ButtonClick.none;
    }
}
