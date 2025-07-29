using System.Collections;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class BuffStaff : StaffBase
{
    [Header("버프 스킬")]
    [SerializeField] private GameObject buffSkillPrefab;
    [SerializeField] private Transform skillOrigin; // 없으면 transform 사용
    [SerializeField] private LayerMask clickMask = ~0;

    private ISkill buffSkill;
    private SkillCooldownBar cooldownBar;


    private InputAction clickAction;
    private bool isShopOpen = false;

    private Coroutine autoAttackRoutine;

    public override void Init(StaffStatsSO stats, RuntimeStaffStatsSO Runtimestats)
    {
        base.Init(stats, Runtimestats);
    }

    void OnEnable()
    {
        // EventBus<ShopUIEvent>.OnEvent += OnShopUIEvent;

        clickAction = new InputAction(type: InputActionType.Button, binding: "<Pointer>/press");
        clickAction.performed += OnPointerPressed;
        clickAction.Enable();
    }

    void OnDisable()
    {
        // EventBus<ShopUIEvent>.OnEvent -= OnShopUIEvent;

        clickAction.performed -= OnPointerPressed;
        clickAction.Disable();
    }

    void Start()
    {
        if (buffSkillPrefab != null)
        {
            var go = Instantiate(buffSkillPrefab, transform);
            buffSkill = go.GetComponent<ISkill>();
        }

        cooldownBar = GetComponentInChildren<SkillCooldownBar>();
        if (cooldownBar != null && buffSkill is ICooldownReadable readable && buffSkill is MonoBehaviour mb)
            cooldownBar.SetSkill(readable, mb);
    }


    private void OnPointerPressed(InputAction.CallbackContext ctx)
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;
        if (isShopOpen) return;

        Vector2 screenPos = Vector2.zero;
        if (Mouse.current != null) screenPos = Mouse.current.position.ReadValue();
        else if (Touchscreen.current != null) screenPos = Touchscreen.current.primaryTouch.position.ReadValue();

        Vector3 world = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0f));
        Vector2 p = new Vector2(world.x, world.y);

        var hits = Physics2D.OverlapPointAll(p, clickMask);
        foreach (var hit in hits)
        {
            if (hit != null && (hit.transform == transform || hit.transform.IsChildOf(transform)))
            {
                TryCastBuff();
                break;
            }
        }
    }

    private void TryCastBuff()
    {
        if (buffSkill == null) return;
        if (!buffSkill.CanCast()) return;

        var origin = skillOrigin != null ? skillOrigin : transform;
        buffSkill.Cast(origin);
    }
}
