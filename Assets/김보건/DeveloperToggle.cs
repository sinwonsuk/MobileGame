using UnityEngine;

public class DeveloperToggle : MonoBehaviour
{
    private float[] speeds = { 1f, 2f, 4f,8f };
    private int currentIndex = 0;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1)) // F1 키로 속도 전환
        {
            currentIndex = (currentIndex + 1) % speeds.Length;
            Time.timeScale = speeds[currentIndex];

            Debug.Log($"[Dev] 게임 속도: {Time.timeScale}배");
        }
    }
}
