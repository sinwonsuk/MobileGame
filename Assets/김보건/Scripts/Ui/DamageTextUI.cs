using UnityEngine;
using TMPro;

public class DamageTextUI : MonoBehaviour
{
    public TextMeshProUGUI text;
    public float floatUpDistance = 1f;
    public float duration = 1f;

    private Vector3 startPos;
    private Vector3 endPos;
    private float elapsed;

    void Awake()
    {
        if (text == null)
            text = GetComponentInChildren<TextMeshProUGUI>();   // 자동 연결

    }
    void Start()
    {
        startPos = transform.position;
        endPos = startPos + Vector3.up * floatUpDistance;
    }

    void Update()
    {
        elapsed += Time.deltaTime;
        float t = elapsed / duration;

        // 위치 위로 부드럽게
        transform.position = Vector3.Lerp(startPos, endPos, t);

        // 알파 점점 줄이기
        if (text != null)
        {
            Color c = text.color;
            c.a = Mathf.Lerp(1f, 0f, t);
            text.color = c;
        }

        if (elapsed >= duration)
        {
            Destroy(gameObject);
        }
    }

    public void Init(string value, Color color)
    {
        if (text != null)
        {
            text.text = value;
            text.color = color;
        }
    }
}