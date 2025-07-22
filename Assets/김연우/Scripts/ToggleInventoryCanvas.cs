
using UnityEngine;

public class ToggleInventoryCanvas : MonoBehaviour
{
    private void Awake()
    {
        EventBus<ToggleInventoryEvent>.OnEvent += OnToggle;
    }

    private void OnDestroy()
    {
        EventBus<ToggleInventoryEvent>.OnEvent -= OnToggle;
    }

    private void OnToggle(ToggleInventoryEvent evt)
    {
        // 이 컴포넌트가 붙어있는 Canvas를 토글
        gameObject.SetActive(!gameObject.activeSelf);
    }
}