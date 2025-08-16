using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class FoodClick : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        customer = GetComponentInParent<Customer>();
        
    }
    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask cookableMask;

    // Update is called once per frame
    void Awake()
    {
        if (!cam) cam = Camera.main;
    }

    void Update()
    {
        if (!TryGetPress(out Vector2 screenPos)) return;
        if (IsPointerOverUI(screenPos)) return;                 // UI 위면 무시


        if (!cam.pixelRect.Contains(screenPos)) return;

        float z = cam.WorldToScreenPoint(transform.position).z;

        Vector3 world = cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, z));

        // 요리 레이어만 포인트 히트(땅/벽 등은 제외)
        var col = Physics2D.OverlapPoint(world, cookableMask);
        if (col == null) return;

        // 내가 맞은 게 ‘이 오브젝트’인지 확인
        if (col.transform != transform) return;

        // === 나머지 조건 ===
        if (Check) return;
        if (customer.customerState != CustomerState.Wait) return;

        EventBus<CookFillamountHandler>.Raise(new CookFillamountHandler(this));

        if (Image.fillAmount < 1f) return;
        if (customer.Slot.NameText.text != foodName) return;

        // 실행
        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.FoodMove, false);
        EventBus<CookMoveHandler>.Raise(new CookMoveHandler(customer.customerTable.transform, customer));
        Destroy(gameObject);
    }

    // 마우스/터치 통합 다운 감지
    bool TryGetPress(out Vector2 screenPos)
    {
        screenPos = default;

        // 실제 활성 터치가 있을 때만 터치 우선
        var ts = Touchscreen.current;
        if (ts != null && ts.touches.Count > 0)
        {
            var t = ts.primaryTouch;
            if (t.press.wasPressedThisFrame)
            {
                screenPos = t.position.ReadValue();
                return true;
            }
        }

        var mouse = Mouse.current;
        if (mouse != null && mouse.leftButton.wasPressedThisFrame)
        {
            screenPos = mouse.position.ReadValue();
            return true;
        }

        return false;
    }

    // 현재 프레임 UI 레이캐스트로 차단
    bool IsPointerOverUI(Vector2 screenPos)
    {
        var es = EventSystem.current;
        if (!es) return false;
        var ped = new PointerEventData(es) { position = screenPos };
        var results = new List<RaycastResult>();
        es.RaycastAll(ped, results);
        return results.Count > 0;
    }

    public void DeleteObject()
    {
        Destroy(gameObject);
    }

    public Image Image { get; set; }
    public string foodName { get; set; }

    Customer customer;

    public bool Check { get; set; } = false;
}
