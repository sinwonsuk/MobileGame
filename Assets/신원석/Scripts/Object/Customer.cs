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

    public Transform Target
    {
        get => target;
        set => target = value;
    }

    float time = 0.0f;

    public CustomerTable customerTable { get; set; }

    private CustomerState customerState = CustomerState.Idle;

    private NavMeshAgent navMeshAgent;

    public void Setup(Transform target)
    {
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
    }

    private void Start()
    {
        firstPosition = transform.position;
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
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
                    time += Time.deltaTime;

                    if (time > 2.0f)
                    {
                        ChangeState(CustomerState.Eat);
                    }
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
}