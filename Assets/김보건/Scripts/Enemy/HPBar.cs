using UnityEngine;

public class HPBar : MonoBehaviour
{
    [SerializeField] private Transform HP_Gauge;

    private Vector3 originalScale;

    public void Awake()
    {
        originalScale = HP_Gauge.localScale;
    }

    public void SetHP(float current, float max)
    {
        float ratio = Mathf.Clamp01(current / max);
        HP_Gauge.localScale = new Vector3(originalScale.x * ratio, originalScale.y, originalScale.z);
    }
}
