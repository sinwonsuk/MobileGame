using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PiggyBank : MonoBehaviour
{
    [Header("초당 누적량")]
    public float ratePerSecond = 1f;

    [Header("화면상 클릭 허용 반경 (픽셀)")]
    public float clickRadiusPixels = 50f;

    [Header("런타임 인테리어 데이터")]
    public RunTimeInteriorData runtimeData; // Manager에서 직접 할당!

    [SerializeField] private int CAP = 1000; // 최대 1000원
    private float accumulated = 0f;

    private TextMeshPro amountText;

    private void Awake()
    {
        amountText = GetComponentInChildren<TextMeshPro>();
        InitializeRuntime(); // 실행 중 전용 초기화
    }

    private void OnEnable()
    {
        // 씬 재활성화 시에도 런타임 초기화 보장하고 싶다면 주석 해제
        // InitializeRuntime();
    }

    public void InitializeRuntime()
    {
        accumulated = 0f;
        if (amountText != null)
            amountText.text = "0";
    }

    void Update()
    {
        // 설치/사용중(isUsed == true)일 때만 누적
        if (runtimeData != null && runtimeData.isUsed)
        {
            // 프레임 누적 (음수·과누적 방지)
            float perFrame = Mathf.Max(0f, ratePerSecond) * Time.deltaTime;
            accumulated = Mathf.Clamp(accumulated + perFrame, 0f, CAP);

            if (amountText != null)
                amountText.text = Mathf.FloorToInt(accumulated).ToString();

            // 터치 수령 처리
            if (Touchscreen.current != null
                && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            {
                Vector2 touchPos = Touchscreen.current.primaryTouch.position.ReadValue();

                Camera cam = Camera.main;
                if (cam != null)
                {
                    Vector3 objScreenPos = cam.WorldToScreenPoint(transform.position);
                    float distSqr = ((Vector2)objScreenPos - touchPos).sqrMagnitude;

                    if (distSqr <= clickRadiusPixels * clickRadiusPixels)
                    {
                        int gain = Mathf.Max(0, Mathf.FloorToInt(accumulated));
                        if (gain > 0)
                        {
                            SoundManager.GetInstance().SfxPlay(SoundManager.sfx.PiggyBank, false);

                            // 돈 지급 이벤트 발생
                            EventBus<MoneyChangePusHandler>.Raise(new MoneyChangePusHandler(gain));

                            // 지급 후 런타임 값만 초기화
                            accumulated = 0f;
                            if (amountText != null) amountText.text = "0";
                        }
                    }
                }
            }
        }
        else
        {
            accumulated = 0f;
            if (amountText != null) amountText.text = "0";
        }
    }
    public void ResetPiggyBankRuntime()
    {
        accumulated = 0f;
        if (amountText != null) amountText.text = "0";
    }
}
