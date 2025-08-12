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

    // ◆ 누적 상한 / 음수 방지
    private const int CAP = 1000; // 최대 1000원
    private float accumulated = 0f;

    private TextMeshPro amountText;
    private const string LastSaveKey = "PiggyBank_LastSave";
    private const string AccumulatedKey = "PiggyBank_Accumulated";

    private void Awake()
    {
        amountText = GetComponentInChildren<TextMeshPro>();
        // 누적금 복원은 RestoreAccumulated()에서만!
    }

    public void RestoreAccumulated()
    {
        if (runtimeData != null && runtimeData.isUsed)
        {
            string lastSaveStr = PlayerPrefs.GetString(LastSaveKey, "");
            accumulated = Mathf.Max(0f, PlayerPrefs.GetFloat(AccumulatedKey, 0f)); // 음수 방지

            if (!string.IsNullOrEmpty(lastSaveStr))
            {
                DateTime lastSaveTime = DateTime.Parse(lastSaveStr);
                TimeSpan elapsed = DateTime.UtcNow - lastSaveTime; // UTC 기준
                // ratePerSecond가 실수로 음수여도 누적은 +방향만
                float delta = Mathf.Max(0f, (float)elapsed.TotalSeconds * Mathf.Max(0f, ratePerSecond));
                accumulated += delta;
            }
        }
        else
        {
            accumulated = 0f;
        }

        // 상한/음수 클램프
        accumulated = Mathf.Clamp(accumulated, 0f, CAP);
        amountText.text = Mathf.FloorToInt(accumulated).ToString();
    }

    void Update()
    {
        // 설치/사용중(isUsed == true)일 때만 누적
        if (runtimeData != null && runtimeData.isUsed)
        {
            // 프레임 누적 (음수·과누적 방지)
            float perFrame = Mathf.Max(0f, ratePerSecond) * Time.deltaTime;
            accumulated = Mathf.Clamp(accumulated + perFrame, 0f, CAP);
            amountText.text = Mathf.FloorToInt(accumulated).ToString();

            if (Touchscreen.current != null
                && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            {
                Vector2 touchPos = Touchscreen.current.primaryTouch.position.ReadValue();
                Vector3 objScreenPos = Camera.main.WorldToScreenPoint(transform.position);
                float distSqr = ((Vector2)objScreenPos - touchPos).sqrMagnitude;

                if (distSqr <= clickRadiusPixels * clickRadiusPixels)
                {
                    int gain = Mathf.FloorToInt(accumulated);
                    // 어떤 경우에도 음수 지급 불가
                    gain = Mathf.Max(0, gain);

                    if (gain > 0)
                    {
                        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.PiggyBank, false);
                        // 돈 지급 이벤트 발생
                        EventBus<MoneyChangePusHandler>
                            .Raise(new MoneyChangePusHandler(gain));

                        // 지급 후 초기화 (저장 포함)
                        accumulated = 0f;
                        SavePiggyBank();
                        amountText.text = "0";
                    }
                }
            }
        }
        else
        {
            accumulated = 0f;
            amountText.text = "0";
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SavePiggyBank();
        }
    }

    private void OnApplicationQuit()
    {
        SavePiggyBank();
    }

    public void ResetPiggyBank()
    {
        accumulated = 0f;
        amountText.text = "0";
        PlayerPrefs.SetFloat(AccumulatedKey, 0f);
        PlayerPrefs.Save();
    }

    private void SavePiggyBank()
    {
        // UTC로 저장 (기존 코드 유지)
        PlayerPrefs.SetString(LastSaveKey, DateTime.UtcNow.ToString());
        // 저장 시에도 클램프 보장
        PlayerPrefs.SetFloat(AccumulatedKey, Mathf.Clamp(accumulated, 0f, CAP));
        PlayerPrefs.Save();
    }
}
