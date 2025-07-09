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
                        ChangeState(CustomerState.Wait);
                    }
                }
                break;
            case CustomerState.Wait:
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        // 포인트를 기준으로 겹친 콜라이더를 찾음
                        Collider2D hit = Physics2D.OverlapPoint(worldPoint);



                        if (hit != null && hit.CompareTag("Food"))
                        {
                            Debug.Log($"[{hit.gameObject.name}] 2D 콜라이더에 마우스 클릭됨");
                        }
                    }

                    spriteRenderer.enabled = true;
                   spriteRenderer.sprite = Slot.IconImage.sprite;                   
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

    public void test()
    {
        // customerTable.transform.position - 
    }


}