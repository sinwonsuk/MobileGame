using UnityEngine;

public class ToggleDungeonCanvas : MonoBehaviour
{
    void Awake()
    {
        EventBus<ToggleDungeonUIEvent>.OnEvent += OnToggle;
        // 다른 UI가 켜지면 닫히도록(원하면 유지)
        EventBus<ToggleInventoryEvent>.OnEvent += _ => CloseIfOpen();
        EventBus<ToggleHunterShopEvent>.OnEvent += _ => CloseIfOpen();
        EventBus<ToggleRestaurantShopEvent>.OnEvent += _ => CloseIfOpen();
    }
    void OnDestroy()
    {
        EventBus<ToggleDungeonUIEvent>.OnEvent -= OnToggle;
        EventBus<ToggleInventoryEvent>.OnEvent -= _ => CloseIfOpen();
        EventBus<ToggleHunterShopEvent>.OnEvent -= _ => CloseIfOpen();
        EventBus<ToggleRestaurantShopEvent>.OnEvent -= _ => CloseIfOpen();
    }
    void Start() => gameObject.SetActive(false);

    private void OnToggle(ToggleDungeonUIEvent _)
    {
        bool next = !gameObject.activeSelf;
        Debug.Log($"[ToggleDungeonCanvas] toggle -> {next}");
        gameObject.SetActive(next);
    }
    private void CloseIfOpen()
    {
        if (gameObject.activeSelf) gameObject.SetActive(false);
    }
}
