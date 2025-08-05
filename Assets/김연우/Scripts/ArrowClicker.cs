using UnityEngine;
using UnityEngine.InputSystem;

public class ArrowClicker : MonoBehaviour
{
    int index;
    public void SetIndex(int i) => index = i;

    [Header("화살표 클릭 허용 반경 (픽셀)")]
    public float clickRadiusPixels = 50f;

    void Update()
    {
        // 터치 (모바일)
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            Vector2 touchPos = Touchscreen.current.primaryTouch.position.ReadValue();
            Vector3 objScreenPos = Camera.main.WorldToScreenPoint(transform.position);

            float distSqr = ((Vector2)objScreenPos - touchPos).sqrMagnitude;
            if (distSqr <= clickRadiusPixels * clickRadiusPixels)
            {
                OnArrowClicked();
            }
        }
        // 마우스 (에디터/PC)
        else if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Vector3 objScreenPos = Camera.main.WorldToScreenPoint(transform.position);

            float distSqr = ((Vector2)objScreenPos - mousePos).sqrMagnitude;
            if (distSqr <= clickRadiusPixels * clickRadiusPixels)
            {
                OnArrowClicked();
            }
        }
    }

    void OnArrowClicked()
    {
        EmployeeManager.Instance.PlaceEmployee(index);
        Debug.Log($"[ArrowClicker] PiggyBank방식 클릭! 위치 index: {index}, 위치 오브젝트: {transform.parent.name}");
    }
}
