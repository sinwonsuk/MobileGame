// ToggleInventoryButton.cs
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ToggleShopButton : MonoBehaviour
{
    public void ToggleSHop()
    {
        Debug.Log("[Shop] 버튼 클릭 감지"); // ← 여기를 확인
        EventBus<ToggleShopEvent>.Raise(new ToggleShopEvent());
    }
}
