
using UnityEngine;

public class ToggleShopCanvas : MonoBehaviour
{
    private void Awake()
    {
        EventBus<ToggleShopEvent>.OnEvent += OnToggle;
    }
    private void Start()
    {
        gameObject.SetActive(false);
    }
    private void OnDestroy()
    {
        EventBus<ToggleShopEvent>.OnEvent -= OnToggle;
    }

    private void OnToggle(ToggleShopEvent evt)
    {
        // 이 컴포넌트가 붙어있는 Canvas를 토글
        gameObject.SetActive(!gameObject.activeSelf);
    }
}