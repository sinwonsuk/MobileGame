using UnityEngine;

public class CameraSlide : MonoBehaviour
{
    private Vector2 startPos;
    private Vector2 endPos;

    private bool isMoving = false;
    private Vector3 targetPos;

    private int currentPage = 0;
    private Vector3[] cameraPositions;

    public float moveSpeed = 5f;
    public float slideThreshold = 100f; // 슬라이드 인식 거리 (픽셀)

    void Start()
    {
        cameraPositions = new Vector3[]
        {
            new Vector3(0f, 0f, -10f),       // 식당
            new Vector3(-73f, 0f, -10f)      // 던전
        };

        targetPos = cameraPositions[0];
    }

    void Update()
    {
        if (!isMoving)
        {
            // 모바일 터치 감지
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                    startPos = touch.position;

                else if (touch.phase == TouchPhase.Ended)
                {
                    endPos = touch.position;
                    HandleSwipe(endPos - startPos);
                }
            }

            // PC 마우스 감지
            else if (Input.GetMouseButtonDown(0))
            {
                startPos = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                endPos = Input.mousePosition;
                HandleSwipe(endPos - startPos);
            }
        }

        // 카메라 이동 처리
        if (isMoving)
        {
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * moveSpeed);

            if (Vector3.Distance(transform.position, targetPos) < 0.05f)
                isMoving = false;
        }
    }

    void HandleSwipe(Vector2 delta)
    {
        if (Mathf.Abs(delta.x) > slideThreshold)
        {
            if (delta.x < 0 && currentPage < cameraPositions.Length - 1)
                currentPage++;
            else if (delta.x > 0 && currentPage > 0)
                currentPage--;

            targetPos = cameraPositions[currentPage];
            isMoving = true;
        }
    }
}
