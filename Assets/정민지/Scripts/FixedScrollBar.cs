using UnityEngine;
using UnityEngine.UI;

public class FixedScrollbar : MonoBehaviour
{
    public Scrollbar scrollbar;
    public float fixedSize = 0.2f; // 원하는 크기 0~1

    void Update()
    {
        scrollbar.size = fixedSize;
    }
}