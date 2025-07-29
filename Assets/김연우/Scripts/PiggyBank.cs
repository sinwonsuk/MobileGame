using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PiggyBank : MonoBehaviour
{
    [Header("초당 누적량")]
    public float ratePerSecond = 1f;
    [Header("화면상 클릭 허용 반경 (픽셀)")]
    public float clickRadiusPixels = 50f;

    private float accumulated = 0f;
    private TextMeshPro amountText;  // 자식 텍스트 컴포넌트
    private void Awake()
    {
        amountText = GetComponentInChildren<TextMeshPro>();
    }
    void Update()
    {
        // 1) 매초 누적
        accumulated += ratePerSecond * Time.deltaTime;
        amountText.text = Mathf.FloorToInt(accumulated).ToString();
        // 2) 터치 입력 감지
        if (Touchscreen.current != null
            && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            // 터치 위치 (스크린 좌표)
            Vector2 touchPos = Touchscreen.current.primaryTouch.position.ReadValue();

            // 오브젝트 월드 좌표 → 스크린 좌표로 변환
            Vector3 objScreenPos = Camera.main.WorldToScreenPoint(transform.position);

            // 스크린 상 거리 제곱 비교 (제곱근 대신 연산 절약)
            float distSqr = ((Vector2)objScreenPos - touchPos).sqrMagnitude;
            if (distSqr <= clickRadiusPixels * clickRadiusPixels)
            {
                int gain = Mathf.FloorToInt(accumulated);
                if (gain > 0)
                {
                    EventBus<MoneyChangePusHandler>
                        .Raise(new MoneyChangePusHandler(gain));
                    accumulated -= gain;
                }
            }
        }
    }
}
