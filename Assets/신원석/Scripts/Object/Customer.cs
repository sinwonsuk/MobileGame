using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;


public enum GenderType
{
    Girl,
    Man,
}

public enum CustomerState
{
    Idle,
    MoveStore,
    Move,
    Wait,
    Eat,
    GoCalculate,
    JoinQueue,
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
        customerSpriteRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);

        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
        foodSpriteRenderer.enabled = false;
        foodOrderSpriteRenderer.enabled = false;
    }

    public void ChangeState(CustomerState customerState)
    {
        this.customerState = customerState;
    }

    float smooth = 10f; // 값이 높을수록 빠르게 반응, 낮을수록 부드럽게
    float smoothedForward = 0f;
    float smoothedRight = 0f;


    void UpdateSortingAndMovementAnim()
    {
        customerSpriteRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);


        Vector3 currentPosition = transform.position;
        Vector3 moveDir = (currentPosition - prevPosition).normalized;
        prevPosition = currentPosition;

        Vector3 forward = transform.up;
        Vector3 right = transform.right;

        float forwardAmount = Vector3.Dot(moveDir, forward);
        float rightAmount = Vector3.Dot(moveDir, right);

        // 여기서 보간 처리
        smoothedForward = Mathf.Lerp(smoothedForward, forwardAmount, Time.deltaTime * smooth);
        smoothedRight = Mathf.Lerp(smoothedRight, rightAmount, Time.deltaTime * smooth);

        animator.SetFloat("Horizontal", smoothedRight);
        animator.SetFloat("Vertical", smoothedForward);
    }

    void SetDestination(Vector3 destination)
    {
        if (navMeshAgent != null)
        {
            navMeshAgent.SetDestination(destination);
        }
    }


    void ChangeIconSprite()
    {
        foodSpriteRenderer.enabled = true;
        foodSpriteRenderer.sprite = Slot.IconImage.sprite;
        foodOrderSpriteRenderer.enabled = true;
    }
    public void PlayGenderVoice(GenderType genderType)
    {
        SoundManager.GetInstance().Sfx_Stop(SoundManager.sfx.Foot);

        if (genderType == GenderType.Girl)
        {
            SoundManager.GetInstance().SfxPlay(SoundManager.sfx.GirlSound, false);
        }
        else if (genderType == GenderType.Man)
        {
            SoundManager.GetInstance().SfxPlay(SoundManager.sfx.ManSound, false);
        }
    }

    private void Update()
    {
        UpdateSortingAndMovementAnim();

        switch (customerState)
        {
            case CustomerState.Idle:
                {
                    EventBus<SitTableHandler>.Raise(new SitTableHandler(this));
                    
                    if (sitTableTransform == null || customerTable == null)
                        return;
                    else
                        ChangeState(CustomerState.MoveStore);
                }
                break;
            case CustomerState.MoveStore:
                {
                    SetDestination(storeEntrancePosition);

                    if (Vector2.Distance(transform.position, storeEntrancePosition) < 0.01f)
                    {
                        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Foot, true);
                        ChangeState(CustomerState.Move);
                        
                        return;
                    }
                }
                break;

            case CustomerState.Move:
                {
                    SetDestination(sitTableTransform.position);

                    if (Vector2.Distance(transform.position, sitTableTransform.position) < 0.01f)
                    {
                        EventBus<CookMakeHandler>.Raise(new CookMakeHandler(Slot.NameText.text, Slot));
                        ChangeIconSprite();
                        PlayGenderVoice(genderType);
                        ChangeState(CustomerState.Wait);
                        return;
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

                        SetDestination(customerManager.counterTransforms[customerManager.customerQueue.Count].transform.position);

                        EventBus<CookDeleteHandler>.Raise(new CookDeleteHandler(this));
                        return;
                    }


                }
                break;
            case CustomerState.GoCalculate:
                {
                    SetDestination(customerManager.counterTransforms[customerManager.customerQueue.Count].transform.position);

                    if (Vector2.Distance(transform.position ,customerManager.counterTransforms[customerManager.customerQueue.Count].transform.position) < 0.01f)
                    {
                        customerManager.EnqueueCustomer(this);
                        ChangeState(CustomerState.JoinQueue);
                        return;
                    }
                }
                break;

            case CustomerState.JoinQueue:
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
                        EventBus<MoneyChangePusHandler>.Raise(new MoneyChangePusHandler(foodPrice));


                        for (int i = 0; i < foodDatas.Count; i++)
                        {
                            if (foodDatas[i].displayName == foodName)
                            {
                                BackendGameData.Instance.AddReputation(foodDatas[i].Getreputation);
                                break;
                            }
                        }

                        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.money, false);
                        return;
                    } 
                }
                break;
            case CustomerState.Back:
                {
                    SetDestination(firstPosition);

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
        }
            
    }



    Animator animator;

    float time = 0.0f;

    private Vector3 prevPosition;
    private Transform target;

    private Vector3 firstPosition;
    private SpriteRenderer customerSpriteRenderer;

    public MenuBoardSlot Slot { get; set; }


    public Transform sitTableTransform
    {
        get => target;
        set => target = value;
    }
    public CustomerTable customerTable { get; set; }

    public CustomerState customerState { get; set; } = CustomerState.Idle;

    public NavMeshAgent navMeshAgent { get; set; }

    [SerializeField] SpriteRenderer foodSpriteRenderer;
    [SerializeField] SpriteRenderer foodOrderSpriteRenderer;


    public int foodPrice { get; set; } // 음식 가격

    public CustomerManager customerManager { get; set; }

    public string foodName { get; set; }

    public Vector3 CalculatePosition;


    private Vector2 storeEntrancePosition = new Vector2(2.04f, 3.24f);

    [SerializeField] List<FoodData> foodDatas = new List<FoodData>();

    [SerializeField] GenderType genderType;

    public List<FoodData> FoodDatas
    {
        get => foodDatas;
        set => foodDatas = value;
    }

}