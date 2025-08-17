using UnityEngine;

public class ToggleHunterShopCanvas : MonoBehaviour
{
    void Awake()
    {
        EventBus<ToggleHunterShopEvent>.OnEvent += OnToggle;
        EventBus<CloseHunterShopEvent>.OnEvent += OnCloseRequested;

        // 레스토랑 샵이 열리면 닫기 (중복 방지)
        EventBus<ToggleRestaurantShopEvent>.OnEvent += OnRestaurantOpened;
        EventBus<ToggleInventoryEvent>.OnEvent += OnInventoryOpened;
    }

    void OnDestroy()
    {
        EventBus<ToggleHunterShopEvent>.OnEvent -= OnToggle;
        EventBus<CloseHunterShopEvent>.OnEvent -= OnCloseRequested;

        EventBus<ToggleRestaurantShopEvent>.OnEvent -= OnRestaurantOpened;
        EventBus<ToggleInventoryEvent>.OnEvent -= OnInventoryOpened;
    }

    void Start() => gameObject.SetActive(false);

    void OnToggle(ToggleHunterShopEvent _)
    {
        gameObject.SetActive(_.Check);
    }

    void OnCloseRequested(CloseHunterShopEvent _)
    {
        if (!gameObject.activeSelf) return;

        gameObject.SetActive(false);
        // Hunter 값이 없으므로 조건 체크 없이 무조건 리셋
        ButtonManager.buttonClick = ButtonClick.none;
    }

    void OnRestaurantOpened(ToggleRestaurantShopEvent _)
    {
        if (!gameObject.activeSelf) return;

        gameObject.SetActive(false);
        // Hunter 값이 없으므로 조건 체크 없이 무조건 리셋
        ButtonManager.buttonClick = ButtonClick.none;
    }
    void OnInventoryOpened(ToggleInventoryEvent _)
    {
        if (!gameObject.activeSelf) return;
        gameObject.SetActive(false);
        ButtonManager.buttonClick = ButtonClick.none;
    }
}
