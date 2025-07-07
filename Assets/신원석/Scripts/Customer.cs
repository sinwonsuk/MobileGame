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
    [SerializeField] private Transform target;

    public Transform Target
    {
        get => target;
        set => target = value;
    }

    private CustomerState customerState = CustomerState.Move;

    private NavMeshAgent navMeshAgent;

    public void Setup(Transform target)
    {
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
    }

    private void Start()
    {
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
                    //navMeshAgent.SetDestination();
                }
                break;
            case CustomerState.Move:
                {
                    //navMeshAgent.SetDestination();
                }
                break;
            case CustomerState.Wait:
                {
                    //navMeshAgent.SetDestination();
                }
                break;
            case CustomerState.Eat:
                {
                    //navMeshAgent.SetDestination();
                }
                break;
            default:
                break;
        }

    }
}