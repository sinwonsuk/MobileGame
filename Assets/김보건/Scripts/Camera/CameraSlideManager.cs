using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CameraSlideManager : baseManager
{
    private CameraSlideManagerConfig config;
    private GameController controller;

    [SerializeField] private float dragSensitivity = 0.005f;

    private Transform Camera => config.cameraTransform;

    // 위치 설정 (식당 던전)
    private Vector3 restaurantPosition = new Vector3(0, 0, -10);
    private Vector3 dungeonPosition = new Vector3(-10, 0, -10);

    private float swipeThreshold = 100f;
    private float slideDuration = 0.5f;

    private Vector2 startPos;
    private Vector2 endPos;
    private Vector3 dragStartCamPos;
    private bool isDragging = false;
    private bool isSliding = false; // 슬라이드중일때 x
    private bool canSlide = false; //던전 켜질때만 슬라이드

    private InputAction pointerPress;
    private InputAction pointerPosition;
    private bool isPointerDown = false;


    public CameraSlideManager(CameraSlideManagerConfig config)
    {
        this.config = config;
    }

    public override void Init()
    {
        if (config.cameraTransform == null)
        {
            var cam = UnityEngine.Camera.main;
            if (cam != null)
            {
                config.cameraTransform = cam.transform;
            }
        }

        EventBus<DungeonSlideToggleEvent>.OnEvent += OnDungeonToggle;

        pointerPress = new InputAction(type: InputActionType.Button, binding: "<Pointer>/press");
        pointerPress.performed += ctx => OnPointerDown();
        pointerPress.canceled += ctx => OnPointerUp();
        pointerPress.Enable();

        pointerPosition = new InputAction(type: InputActionType.PassThrough, binding: "<Pointer>/position");
        pointerPosition.Enable();


    }
    ~CameraSlideManager()
    {
        EventBus<DungeonSlideToggleEvent>.OnEvent -= OnDungeonToggle;

        pointerPress.performed -= ctx => OnPointerDown();
        pointerPress.canceled -= ctx => OnPointerUp();
        pointerPress.Disable();
        pointerPosition.Disable();
    }

    public override void GetController(GameController controller)
    {
        this.controller = controller;
    }

    public override void Update()
    {
        if (isSliding || !canSlide) return;
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

        Vector2 pointerPos = pointerPosition.ReadValue<Vector2>();

        if (isPointerDown)
        {
            if (!isDragging)
            {
                startPos = pointerPos;
                dragStartCamPos = Camera.position;
                isDragging = true;
            }
            else
            {
                Vector2 delta = pointerPos - startPos;
                Vector3 dragPos = dragStartCamPos - new Vector3(delta.x * dragSensitivity, 0f, 0f);
                dragPos.x = Mathf.Clamp(dragPos.x, dungeonPosition.x, restaurantPosition.x);
                Camera.position = dragPos;
            }
        }

//#if UNITY_EDITOR || UNITY_STANDALONE
//        if (Input.GetMouseButtonDown(0))
//        {
//            startPos = Input.mousePosition;
//            dragStartCamPos = Camera.position;
//            isDragging = true;
//        }
//        else if (Input.GetMouseButton(0) && isDragging)
//        {
//            Vector2 delta = (Vector2)Input.mousePosition - startPos;
//            Vector3 dragPos = dragStartCamPos - new Vector3(delta.x * dragSensitivity, 0f, 0f);
//            dragPos.x = Mathf.Clamp(dragPos.x, dungeonPosition.x, restaurantPosition.x);
//            Camera.position = dragPos;
//        }
//        else if (Input.GetMouseButtonUp(0) && isDragging)
//        {
//            isDragging = false;
//            endPos = Input.mousePosition;
//            HandleSwipe(endPos - startPos);
//        }
//#else
//        if (Input.touchCount > 0)
//        {
//            Touch touch = Input.GetTouch(0);

//            if (touch.phase == TouchPhase.Began)
//            {
//                startPos = touch.position;
//                dragStartCamPos = Camera.position;
//                isDragging = true;
//            }
//            else if (touch.phase == TouchPhase.Moved && isDragging)
//            {
//                Vector2 delta = touch.position - startPos;
//                Vector3 dragPos = dragStartCamPos - new Vector3(delta.x * dragSensitivity, 0f, 0f);
//                dragPos.x = Mathf.Clamp(dragPos.x, dungeonPosition.x, restaurantPosition.x);
//                Camera.position = dragPos;
//            }
//            else if (touch.phase == TouchPhase.Ended && isDragging)
//            {
//                isDragging = false;
//                endPos = touch.position;
//                HandleSwipe(endPos - startPos);
//            }
//        }
//#endif
    }

    private void OnPointerDown()
    {
        isPointerDown = true;
    }

    private void OnPointerUp()
    {
        isPointerDown = false;

        if (isDragging)
        {
            isDragging = false;
            Vector2 endPos = pointerPosition.ReadValue<Vector2>();
            HandleSwipe(endPos - startPos);
        }
    }

    private void HandleSwipe(Vector2 delta)
    {
        float distance = delta.x;

        if (Mathf.Abs(distance) > swipeThreshold)
        {
            if (distance < 0)
                MoveToRestaurant(); 
            else
                MoveToDungeon();  
        }
        else
        {
            // 거리 짧으면 현재 위치 기준 가까운 쪽으로 이동
            float middleX = (restaurantPosition.x + dungeonPosition.x) * 0.5f;
            if (Camera.position.x < middleX)
                MoveToDungeon();
            else
                MoveToRestaurant();
        }
    }

    private IEnumerator SlideCamera(Vector3 target)
    {
        isSliding = true;

        Vector3 start = Camera.position;
        float elapsed = 0f;

        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / slideDuration);
            Camera.position = Vector3.Lerp(start, target, t);
            yield return null;
        }

        Camera.position = target;

        isSliding = false;
    }

    public void MoveToDungeon()
    {
        if (controller != null)
            controller.StartCoroutine(SlideCamera(dungeonPosition));
    }

    public void MoveToRestaurant()
    {
        if (controller != null)
            controller.StartCoroutine(SlideCamera(restaurantPosition));
    }


    private void OnDungeonToggle(DungeonSlideToggleEvent evt)
    {
        canSlide = evt.isDungeonActive;
    }

    public void TemporarilyDisableSlide(float seconds)
    {
        canSlide = false;
        controller.StartCoroutine(EnableSlideAfterDelay(seconds));
    }

    private IEnumerator EnableSlideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        canSlide = true;
    }

    public override void Destory()
    {

    }
}
