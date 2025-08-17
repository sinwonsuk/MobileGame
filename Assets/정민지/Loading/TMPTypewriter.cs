using TMPro;
using UnityEngine;

public class TMPTypewriter : MonoBehaviour
{
    public TextMeshProUGUI tmp;
    public float duration = 1f; // 마지막 3글자 타이핑 속도
    public float waitTime = 0.5f; // 마지막 3글자 다 보여준 후 잠시 대기

    private string fullText;
    private string prefix;
    private string suffix;

    void OnEnable()
    {
        fullText = tmp.text;

        if (fullText.Length <= 3)
        {
            prefix = "";
            suffix = fullText;
        }
        else
        {
            prefix = fullText.Substring(0, fullText.Length - 3); // 앞 글자
            suffix = fullText.Substring(fullText.Length - 3);     // 마지막 3글자
        }

        tmp.text = prefix; // 처음에는 앞 글자만 표시
        StartCoroutine(AnimateSuffix());
    }

    System.Collections.IEnumerator AnimateSuffix()
    {
        int total = suffix.Length;

        while (true)
        {
            tmp.text = prefix;
            float time = 0;

            while (time < duration)
            {
                int count = Mathf.FloorToInt(Mathf.Lerp(0, total, time / duration));
                tmp.text = prefix + suffix.Substring(0, count);
                time += Time.deltaTime;
                yield return null;
            }

            tmp.text = prefix + suffix;
            yield return new WaitForSeconds(waitTime);
        }
    }
}