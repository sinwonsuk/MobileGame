// ToggleInventoryButton.cs
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ToggleInventoryButton : MonoBehaviour
{
    public void ToggleInventory()
    {
        Debug.Log("[ToggleInventory] 버튼 클릭 감지"); // ← 여기를 확인
        EventBus<ToggleInventoryEvent>.Raise(new ToggleInventoryEvent());
    }
}
