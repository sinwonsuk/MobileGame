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

    private float accumulated = 0f;
    private TextMeshPro amountText;
    private const string LastSaveKey = "PiggyBank_LastSave";
    private const string AccumulatedKey = "PiggyBank_Accumulated";

    private void Awake()
    {
        amountText = GetComponentInChildren<TextMeshPro>();
        // 마지막 저장 시각 불러오기
        string lastSaveStr = PlayerPrefs.GetString(LastSaveKey, "");
        accumulated = PlayerPrefs.GetFloat(AccumulatedKey, 0f);

        if (!string.IsNullOrEmpty(lastSaveStr))
        {
            DateTime lastSaveTime = DateTime.Parse(lastSaveStr);
            TimeSpan elapsed = DateTime.UtcNow - lastSaveTime;
            // 누적
            accumulated += (float)elapsed.TotalSeconds * ratePerSecond;
        }
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
            Vector2 touchPos = Touchscreen.current.primaryTouch.position.ReadValue();
            Vector3 objScreenPos = Camera.main.WorldToScreenPoint(transform.position);
            float distSqr = ((Vector2)objScreenPos - touchPos).sqrMagnitude;
            if (distSqr <= clickRadiusPixels * clickRadiusPixels)
            {
                int gain = Mathf.FloorToInt(accumulated);
                if (gain > 0)
                {
                    EventBus<MoneyChangePusHandler>
                        .Raise(new MoneyChangePusHandler(gain));
                    accumulated = 0f;
                    SavePiggyBank(); // 회수 시점에도 저장
                }
            }
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

    private void SavePiggyBank()
    {
        PlayerPrefs.SetString(LastSaveKey, DateTime.UtcNow.ToString());
        PlayerPrefs.SetFloat(AccumulatedKey, accumulated);
        PlayerPrefs.Save();
    }
}
