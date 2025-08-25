using UnityEngine;
using TMPro;

public interface IRemainingTimeReadable
{
    float GetRemainingSeconds();
}

public class SkillCooldownBarWithText : MonoBehaviour
{
    public enum Anchor { Left, Center, Right }

    [Header("Fill (SpriteRenderer scale X)")]
    [SerializeField] private Transform fillTransform;

    [Header("Text (TMP 3D)")]
    [SerializeField] private TMP_Text label;

    [Header("Target (ICooldownReadable + IRemainingTimeReadable)")]
    [SerializeField] private MonoBehaviour target;

    [Header("Shrink Anchor (피벗 대체)")]
    [SerializeField] private Anchor anchor = Anchor.Right; // 오른쪽 고정

    [Header("Sorting 옵션")]
    [SerializeField] private int labelOrderOffset = 1;           // Fill보다 몇 단계 앞?
    [SerializeField] private bool followFillSortingEveryFrame = true; // 매 프레임 추적

    private ICooldownReadable ratioSrc;
    private IRemainingTimeReadable timeSrc;

    private Vector3 initialScale;
    private Vector3 initialLocalPos;
    private SpriteRenderer fillSR;
    private float baseLocalWidth;
    private Renderer labelRenderer;

    void Awake()
    {
        if (fillTransform != null)
        {
            initialScale = fillTransform.localScale;
            initialLocalPos = fillTransform.localPosition;
            fillSR = fillTransform.GetComponent<SpriteRenderer>();
            baseLocalWidth = CalcBaseLocalWidth();
        }

        if (label != null) labelRenderer = label.GetComponent<Renderer>();
        if (target != null)
        {
            ratioSrc = target as ICooldownReadable;
            timeSrc = target as IRemainingTimeReadable;
        }

        ApplySorting();
    }

    void Start()
    {
        // 프리팹이 씬에 놓인 뒤 최종 정렬 재보정
        ApplySorting();
    }

    private float CalcBaseLocalWidth()
    {
        if (fillSR == null || fillSR.sprite == null) return 1f;
        if (fillSR.drawMode != SpriteDrawMode.Simple)
            return fillSR.size.x * initialScale.x;                 // Sliced/Tiled
        else
            return fillSR.sprite.bounds.size.x * initialScale.x;   // Simple
    }

    private void ApplySorting()
    {
        if (labelRenderer == null) return;

        if (fillSR != null)
        {
            labelRenderer.sortingLayerID = fillSR.sortingLayerID;
            int targetOrder = fillSR.sortingOrder + labelOrderOffset;
            if (labelRenderer.sortingOrder != targetOrder)
                labelRenderer.sortingOrder = targetOrder;
        }

        // 깊이 테스트로 가려질 수 있으면 살짝 앞으로
        var lp = label.transform.localPosition;
        if (lp.z >= -0.001f) label.transform.localPosition = new Vector3(lp.x, lp.y, -0.001f);
    }

    public void SetTarget(MonoBehaviour mb)
    {
        target = mb;
        ratioSrc = mb as ICooldownReadable;
        timeSrc = mb as IRemainingTimeReadable;
        ApplySorting(); // 런타임에 바꿔도 즉시 재정렬
    }

    void Update()
    {
        if (ratioSrc != null && fillTransform != null)
        {
            float r = Mathf.Clamp01(ratioSrc.GetCooldownRatio());
            fillTransform.localScale = new Vector3(
                initialScale.x * r, initialScale.y, initialScale.z
            );

            // 에디터 Pivot없이 오른/왼/센터 고정
            float dx = 0f;
            switch (anchor)
            {
                case Anchor.Right: dx = +baseLocalWidth * (1f - r) * 0.5f; break;
                case Anchor.Left: dx = -baseLocalWidth * (1f - r) * 0.5f; break;
                case Anchor.Center: dx = 0f; break;
            }
            fillTransform.localPosition = new Vector3(
                initialLocalPos.x + dx, initialLocalPos.y, initialLocalPos.z
            );
        }

        if (label != null && timeSrc != null)
        {
            float sec = Mathf.Max(0f, timeSrc.GetRemainingSeconds());
            int isec = Mathf.CeilToInt(sec);
            int m = isec / 60, s = isec % 60;
            label.text = (m > 0) ? $"{m:00}:{s:00}" : $"{s:00}";
        }
    }

    void LateUpdate()
    {
        if (followFillSortingEveryFrame) ApplySorting();
    }
}
