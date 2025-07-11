using UnityEngine;

public class DeveloperToggle : MonoBehaviour
{
    private float[] speeds = { 1f, 2f, 4f,8f };
    private int currentIndex = 0;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1)) // F1 Ű�� �ӵ� ��ȯ
        {
            currentIndex = (currentIndex + 1) % speeds.Length;
            Time.timeScale = speeds[currentIndex];

            Debug.Log($"[Dev] ���� �ӵ�: {Time.timeScale}��");
        }
    }
}
