using UnityEngine;
using System;

public class HPBar : MonoBehaviour
{
    [SerializeField] private Transform HP_Gauge;

    // 원래 스케일을 double 로 보관
    private double originalScaleX;
    private double originalScaleY;
    private double originalScaleZ;

    public void Awake()
    {
        Vector3 s = HP_Gauge.localScale;
        originalScaleX = s.x;
        originalScaleY = s.y;
        originalScaleZ = s.z;
    }

    public void SetHP(double current, double max)
    {
        double ratio = (max > 0.0) ? current / max : 0.0;
        ratio = Math.Max(0.0, Math.Min(1.0, ratio));
        HP_Gauge.localScale = new Vector3(
            (float)(originalScaleX * ratio),
            (float)originalScaleY,
            (float)originalScaleZ
        );
    }
}
