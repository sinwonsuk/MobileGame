using UnityEngine;

public class ToggleInventoryCanvas : MonoBehaviour
{
    void Awake()
    {
        EventBus<ToggleInventoryEvent>.OnEvent += OnToggle;
        EventBus<CloseInventoryEvent>.OnEvent += OnCloseRequested;

        // ¡Ú ÇåÅÍ ¼¥ÀÌ ÄÑÁö¸é ³ª´Â ´Ý±â (ÇåÅÍ¿ÍÀÇ Áßº¹ ¹æÁö)
        EventBus<ToggleHunterShopEvent>.OnEvent += OnHunterOpened;
    }

    void OnDestroy()
    {
        EventBus<ToggleInventoryEvent>.OnEvent -= OnToggle;
        EventBus<CloseInventoryEvent>.OnEvent -= OnCloseRequested;

        EventBus<ToggleHunterShopEvent>.OnEvent -= OnHunterOpened;
    }

    void Start() => gameObject.SetActive(false);

    void OnToggle(ToggleInventoryEvent _)
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    void OnCloseRequested(CloseInventoryEvent _)
    {
        if (!gameObject.activeSelf) return;

        gameObject.SetActive(false);
        if (ButtonManager.buttonClick == ButtonClick.Inven)
            ButtonManager.buttonClick = ButtonClick.none;
    }

    // ¡Ú ÇåÅÍ ¼¥ ¿­¸² ¡æ ÀÎº¥ ´Ý±â
    void OnHunterOpened(ToggleHunterShopEvent _)
    {
        if (!gameObject.activeSelf) return;

        gameObject.SetActive(false);
        if (ButtonManager.buttonClick == ButtonClick.Inven)
            ButtonManager.buttonClick = ButtonClick.none;
    }
}
