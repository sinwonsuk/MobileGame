using UnityEngine;
using UnityEngine.AI;

// Change the access modifier of CustomerState to public to fix CS0051
public enum CustomerState
{
    Idle,
    Move,
    Wait,
    Eat
}


public class Customer : MonoBehaviour
{
    private void OnEnable()
    {
        EventBus<CustomerStateChangeHandler>.OnEvent += ChangeState;
    }

    private void OnDisable()
    {
        EventBus<CustomerStateChangeHandler>.OnEvent -= ChangeState;
    }



    private void Start()
    {
        firstPosition = transform.position;
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
        spriteRenderer.enabled = false;
    }

    public void ChangeState(CustomerState customerState)
    {
        this.customerState = customerState;
    }
    private void Update()
    {

        switch (customerState)
        {
            case CustomerState.Idle:
                {
                    EventBus<SitTableHandler>.Raise(new SitTableHandler(this));
                    

                    if (Target == null || customerTable == null)
                        return;
                    else
                        ChangeState(CustomerState.Move);
                }
                break;
            case CustomerState.Move:
                {
                    navMeshAgent.SetDestination(Target.position);

                    if (Vector2.Distance(transform.position, Target.position) < 0.01f)
                    {
                        EventBus<CookMakeHandler>.Raise(new CookMakeHandler(Slot.NameText.text, Slot));
                        spriteRenderer.enabled = true;
                        spriteRenderer.sprite = Slot.IconImage.sprite;
                        ChangeState(CustomerState.Wait);
                    }
                }
                break;
            case CustomerState.Wait:
                {
                    
                }
                break;
            case CustomerState.Eat:
                {
                   

                    navMeshAgent.SetDestination(firstPosition);

                    if (Vector2.Distance(firstPosition, transform.position) < 0.01f)
                    {
                        customerTable.IsSittingAtTable = false;
                        Destroy(gameObject);
                    }
                }
                break;

            default:
                break;
        }

    }

    public void ChangeState(CustomerStateChangeHandler customerStateChangeHandler)
    {
        if (customerStateChangeHandler.customer == this)
        {
            customerState = customerStateChangeHandler.customerState;
            EventBus<MenuReduceHandler>.Raise(new MenuReduceHandler(Slot));
            EventBus<CookDeleteHandler>.Raise(new CookDeleteHandler(this));
        }
            
    }



    private Transform target;

    private Vector3 firstPosition;

    public MenuBoardSlot Slot { get; set; }

    public Transform Target
    {
        get => target;
        set => target = value;
    }
    public CustomerTable customerTable { get; set; }

    public CustomerState customerState { get; set; } = CustomerState.Idle;

    private NavMeshAgent navMeshAgent;

    [SerializeField] SpriteRenderer spriteRenderer;
}