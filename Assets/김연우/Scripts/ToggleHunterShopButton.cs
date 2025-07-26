
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ToggleHunterShopButton : MonoBehaviour
{
    public void ToggleSHop()
    {
        Debug.Log("[Shop11] 버튼 클릭 감지"); // ← 여기를 확인
        EventBus<ToggleHunterShopEvent>.Raise(new ToggleHunterShopEvent());
        Debug.Log("[Shop11] 이벤트 Raise 완료");
    }
}
