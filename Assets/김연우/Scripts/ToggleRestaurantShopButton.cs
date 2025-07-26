using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ToggleRestaurantShop : MonoBehaviour
{
    public void ToggleSHop()
    {
        Debug.Log("[Shop22] 버튼 클릭 감지"); // ← 여기를 확인
        EventBus<ToggleRestaurantShopEvent>.Raise(new ToggleRestaurantShopEvent());
        Debug.Log("[Shop22] 이벤트 Raise 완료");
    }
}
