using UnityEngine;

public class HPBar : MonoBehaviour
{
    [SerializeField] private Transform HP_Gauge; 

    public void SetHP(float current, float max)
    {
        float ratio = Mathf.Clamp01(current / max);
        HP_Gauge.localScale = new Vector3(ratio, 1, 1);
    }
}
