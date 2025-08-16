using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider2D))]
public class ArrowClicker : MonoBehaviour
{
    int index;
    public void SetIndex(int i) => index = i;

    [Header("UI 위 클릭 무시")]
    public bool ignoreWhenOverUI = true;

    [Header("콜라이더 딱맞게(OverlapPoint) / 확장 반경 사용")]
    public bool useExpandedRadius = false;

    [Header("확장 반경(픽셀) - useExpandedRadius=true일 때 사용")]
    public float extraRadiusPixels = 50f;

    [Header("클릭 레이어 마스크")]
    public LayerMask clickableMask = ~0;

    Camera cam;
    Collider2D col2D;

    void Awake()
    {
        cam = Camera.main;
        col2D = GetComponent<Collider2D>();
    }

    void Update()
    {
        Vector2 screenPos;
        if (!TryGetPressedPosition(out screenPos)) return;

        if (ignoreWhenOverUI && EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject())
            return;

        // 1) 스크린 좌표 유효성 체크
        if (float.IsNaN(screenPos.x) || float.IsNaN(screenPos.y) ||
            float.IsInfinity(screenPos.x) || float.IsInfinity(screenPos.y))
        {
            return; // 비정상 입력 무시
        }

        if (cam == null) cam = Camera.main;
        if (cam == null) return; // 카메라가 진짜 없으면 중단

        // 2) 안전한 월드 좌표 변환
        Vector2 worldPoint;
        if (!TryScreenToWorldSafe(cam, screenPos, col2D.bounds.center.z, out worldPoint))
        {
            // 3) 최후 수단: 레이캐스트로 콜라이더 히트 체크
            if (TryRayHit2D(cam, screenPos, out var hit2D))
            {
                if (hit2D.collider && (hit2D.transform == transform || hit2D.transform.IsChildOf(transform)))
                {
                    OnArrowClicked();
                }
            }
            return;
        }

        // 4) 콜라이더 판정
        if (!useExpandedRadius)
        {
            if (col2D.OverlapPoint(worldPoint))
            {
                OnArrowClicked();
                return;
            }

            var hit = Physics2D.OverlapPoint(worldPoint, clickableMask);
            if (hit && (hit.transform == transform || hit.transform.IsChildOf(transform)))
                OnArrowClicked();
        }
        else
        {
            float zDist = Mathf.Abs(col2D.bounds.center.z - cam.transform.position.z);
            float rWorld = PixelsToWorld(cam, extraRadiusPixels, zDist);
            var hit = Physics2D.OverlapCircle(worldPoint, rWorld, clickableMask);
            if (hit && (hit.transform == transform || hit.transform.IsChildOf(transform)))
                OnArrowClicked();
        }
    }

    bool TryGetPressedPosition(out Vector2 pos)
    {
        // 터치
        if (Touchscreen.current != null &&
            Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            pos = Touchscreen.current.primaryTouch.position.ReadValue();
            return true;
        }
        // 마우스
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            pos = Mouse.current.position.ReadValue();
            return true;
        }
        pos = default;
        return false;
    }

    // ScreenToWorldPoint 안전 래퍼 (직교/원근 둘 다 OK)
    bool TryScreenToWorldSafe(Camera c, Vector2 sp, float targetZ, out Vector2 world)
    {
        world = default;

        // 원근 카메라면 0보다 큰 z가 필요. 대상 콜라이더 z와 카메라 z 거리 사용
        float zDist = Mathf.Abs(targetZ - c.transform.position.z);
        if (!c.orthographic)
        {
            if (zDist < 0.01f) zDist = 0.01f; // 0 회피
        }
        else
        {
            // 직교는 z 무시됨. 그래도 안전하게 0이상으로.
            zDist = Mathf.Max(zDist, 0.0f);
        }

        Vector3 inVec = new Vector3(sp.x, sp.y, zDist);

        // 방어적: Infinity/NaN 방지
        if (float.IsNaN(inVec.x) || float.IsNaN(inVec.y) || float.IsNaN(inVec.z) ||
            float.IsInfinity(inVec.x) || float.IsInfinity(inVec.y) || float.IsInfinity(inVec.z))
            return false;

        Vector3 w = c.ScreenToWorldPoint(inVec);

        if (float.IsNaN(w.x) || float.IsNaN(w.y) || float.IsInfinity(w.x) || float.IsInfinity(w.y))
            return false;

        world = w;
        return true;
    }

    bool TryRayHit2D(Camera c, Vector2 sp, out RaycastHit2D hit)
    {
        var ray = c.ScreenPointToRay(sp);
        // 2D 전용: 카메라 정면으로 충분히 길게 쏨
        hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity, clickableMask);
        return hit.collider != null;
    }

    float PixelsToWorld(Camera c, float px, float zDist)
    {
        // 같은 z 깊이에서 px만큼의 스크린 픽셀 → 월드 유닛 근사
        Vector3 a = c.ScreenToWorldPoint(new Vector3(0, 0, zDist));
        Vector3 b = c.ScreenToWorldPoint(new Vector3(px, 0, zDist));
        return Mathf.Abs(b.x - a.x);
    }

    void OnArrowClicked()
    {
        EmployeeManager.Instance.PlaceEmployee(index);
        Debug.Log($"[ArrowClicker] Collider 방식 클릭! index: {index}, obj: {transform.parent?.name ?? name}");
    }
}
