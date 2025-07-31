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
            accumulated = PlayerPrefs.GetFloat(AccumulatedKey, 0f);

            if (!string.IsNullOrEmpty(lastSaveStr))
            {
                DateTime lastSaveTime = DateTime.Parse(lastSaveStr);
                TimeSpan elapsed = DateTime.UtcNow - lastSaveTime;
                accumulated += (float)elapsed.TotalSeconds * ratePerSecond;
            }
        }
        else
        {
            accumulated = 0f;
        }
        amountText.text = Mathf.FloorToInt(accumulated).ToString();
    }

    void Update()
    {
        // 설치/사용중(isUsed == true)일 때만 누적
        if (runtimeData != null && runtimeData.isUsed)
        {
            accumulated += ratePerSecond * Time.deltaTime;
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
                    if (gain > 0)
                    {
                        // 돈 지급 이벤트 발생
                        EventBus<MoneyChangePusHandler>
                            .Raise(new MoneyChangePusHandler(gain));
                        accumulated = 0f;
                        SavePiggyBank();
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
        PlayerPrefs.SetString(LastSaveKey, DateTime.UtcNow.ToString());
        PlayerPrefs.SetFloat(AccumulatedKey, accumulated);
        PlayerPrefs.Save();
    }
}
