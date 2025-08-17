using TMPro;
using UnityEngine;

public class TMPTypewriter : MonoBehaviour
{
    public TextMeshProUGUI tmp;
    public float duration = 2f;
    public float waitTime = 1f; // 텍스트가 다 나온 후 잠시 멈춤

    void OnEnable()
    {
        StartCoroutine(AnimateLoop());
    }

    System.Collections.IEnumerator AnimateLoop()
    {
        int total = tmp.text.Length;

        while (true) // 무한 반복
        {
            tmp.maxVisibleCharacters = 0;
            float time = 0;

            while (time < duration)
            {
                int count = Mathf.FloorToInt(Mathf.Lerp(0, total, time / duration));
                tmp.maxVisibleCharacters = count;
                time += Time.deltaTime;
                yield return null;
            }

            tmp.maxVisibleCharacters = total;
            yield return new WaitForSeconds(waitTime); // 다 보여준 후 잠시 대기
        }
    }
}