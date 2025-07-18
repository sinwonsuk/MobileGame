using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

// Change the access modifier of CustomerState to public to fix CS0051
public enum CustomerState
{
    Idle,
    Move,
    Wait,
    Eat,
    GoCalculate,
    GoCalculate2,
    calculate,
    Back,
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
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        customerSpriteRenderer = GetComponent<SpriteRenderer>();


        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
        foodSpriteRenderer.enabled = false;
    }

    public void ChangeState(CustomerState customerState)
    {
        this.customerState = customerState;
    }
    private void Update()
    {
        customerSpriteRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);
        Vector3 currentPosition = transform.position;
        Vector3 moveDir = (currentPosition - prevPosition).normalized;
        prevPosition = currentPosition;

        Vector3 forward = transform.up;
        Vector3 right = transform.right;

        float forwardAmount = Vector3.Dot(moveDir, forward); 
        float rightAmount = Vector3.Dot(moveDir, right);    

        animator.SetFloat("Horizontal", rightAmount);
        animator.SetFloat("Vertical", forwardAmount);


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
                        foodSpriteRenderer.enabled = true;
                        foodSpriteRenderer.sprite = Slot.IconImage.sprite;
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
                    animator.SetBool("Eat",true);

                    time += Time.deltaTime;

                    if(time > 2.0f)
                    {
                        time = 0.0f;
                        ChangeState(CustomerState.GoCalculate);
                        customerTable.IsSittingAtTable = false;
                        animator.SetBool("Back", true);


                        //customerManager.EnqueueCustomer(this);

                        navMeshAgent.SetDestination(customerManager.counterTransforms[customerManager.customerQueue.Count].transform.position);
                        return;
                    }


                }
                break;
            case CustomerState.GoCalculate:
                {
                    navMeshAgent.SetDestination(customerManager.counterTransforms[customerManager.customerQueue.Count].transform.position);


                    if (Vector2.Distance(transform.position ,customerManager.counterTransforms[customerManager.customerQueue.Count].transform.position) < 0.01f)
                    {
                        customerManager.EnqueueCustomer(this);
                        ChangeState(CustomerState.GoCalculate2);
                        return;
                    }
                }
                break;

            case CustomerState.GoCalculate2:
                {
                    //customerManager.UpdateQueueDestinations();

                    if (Vector2.Distance(transform.position, customerManager.counterTransforms[0].transform.position) < 0.01f)
                    {
                        ChangeState(CustomerState.calculate);
                        return;
                    }
                }
                break;
            case CustomerState.calculate:
                {
                    time += Time.deltaTime;

                    if (time > 5.0f)
                    {
                        time = 0.0f;
                        customerManager.DequeueCustomer();
                        ChangeState(CustomerState.Back);
                        return;
                    }
                }
                break;
            case CustomerState.Back:
                {
                    navMeshAgent.SetDestination(firstPosition);

                    if (Vector2.Distance(firstPosition, transform.position) < 0.01f)
                    {
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



    Animator animator;

    float time = 0.0f;

    private Vector3 prevPosition;
    private Transform target;

    private Vector3 firstPosition;
    private SpriteRenderer customerSpriteRenderer;

    public MenuBoardSlot Slot { get; set; }


    public Transform Target
    {
        get => target;
        set => target = value;
    }
    public CustomerTable customerTable { get; set; }

    public CustomerState customerState { get; set; } = CustomerState.Idle;

    public NavMeshAgent navMeshAgent { get; set; }

    [SerializeField] SpriteRenderer foodSpriteRenderer;

    [SerializeField] List<AnimatorController> animatorControllers;

    [SerializeField] List<Sprite> sprites;



    public List<AnimatorController> AnimatorControllers
    {         
        get => animatorControllers;
    }
    public List<Sprite> Sprites
    {
        get => sprites;
    }

    public CustomerManager customerManager { get; set; }

    public Vector3 CalculatePosition;

}