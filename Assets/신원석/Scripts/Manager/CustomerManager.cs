using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.Rendering.DebugUI;
using static UnityEngine.RuleTile.TilingRuleOutput;
using Transform = UnityEngine.Transform;

public class CustomerManager : baseManager, IGameManager
{

    public CustomerManager(CustomerManagerConfig config)
    {
        conFig = config;

        EventBus<CustomerSpawnHandler>.OnEvent += SpawnCustomer;
        EventBus<GetCusomersEvent>.OnEvent += GetCustomerEvent;
    }

    public CustomerManager(BaseScriptableObject baseScriptableObject)
    {
        type = typeof(CustomerManager);
        conFig = (CustomerManagerConfig)baseScriptableObject;
    }

    ~CustomerManager()
    {
        EventBus<CustomerSpawnHandler>.OnEvent -= SpawnCustomer;
        EventBus<GetCusomersEvent>.OnEvent -= GetCustomerEvent;
    }
    
    public override void Init()
    {
        EventBus<MenuLoadedEvent>.Raise(new MenuLoadedEvent(this));


        counterTransforms = GameObject.FindWithTag("Counter").GetComponent<OrderCounter>().QueuePositions;
    }

    public void GetCustomerEvent(GetCusomersEvent getCustomersEvent)
    {
        getCustomersEvent.employee.customers = customers;
    }


    public void SpawnCustomer(CustomerSpawnHandler customerSpawnHandler)
    {
        for (int i = 0; i < conFig.GetGameObjects().Count; i++)
        {
            GameObject.Instantiate(conFig.GetGameObjects()[i]);
        }
    }

    public override void Update()
    {
        if (coroutine == null && MenuBoardSlots.Count != 0)
            coroutine = controller.StartCoroutine(CheckMenuRoutine());
    }

    public override void GetController(GameController gameController)
    {
        this.controller = gameController;
    }

    public IEnumerator CheckMenuRoutine()
    {
        EventBus<ManagementActiveCheckHandler>.Raise(new ManagementActiveCheckHandler(ClickType.FoodSlot, this));

        if (isActive == false)
        {

            while (true)
            {
                yield return new WaitForSeconds(2.0f);


                if (MenuBoardSlots.Count != 0)
                {
                    CheckMenu();
                }

                yield return null;
            }
        }
    }

    public void EnqueueCustomer(Customer customer)
    {
        if (customerQueue.Count < counterTransforms.Count)
        {
            customerQueue.Enqueue(customer);
            UpdateQueueDestinations();
        }
        else
        {
            //customer.WaitOutside();
        }
    }

    public void GetCustomers()

    {

    }

    private void UpdateQueueDestinations()
    {
        int idx = 0;
        foreach (Customer cust in customerQueue)
        {
           
            int posIndex = Mathf.Min(idx, counterTransforms.Count - 1);
            cust.navMeshAgent.SetDestination(counterTransforms[posIndex].position);
            cust.CalculatePosition = counterTransforms[posIndex].position; 
            idx++;          
        }
    }
    public void DequeueCustomer()
    {
        if (customerQueue.Count > 0)
        {
            customerQueue.Dequeue();
            UpdateQueueDestinations();
        }
    }

    public void CheckMenu()
    {
        EventBus<RandomMenuSelectionHandler>.Raise(new RandomMenuSelectionHandler(this));

        if (Slot == null)
            return;

        GameObject obj = GameObject.Instantiate(conFig.GetGameObjects()[0]);

        Customer customer = obj.GetComponent<Customer>();

        int random = Random.Range(0,customer.AnimatorControllers.Count);

        customer.GetComponent<Animator>().runtimeAnimatorController = customer.AnimatorControllers[random];
        customer.GetComponent<SpriteRenderer>().sprite = customer.Sprites[random];


        customers.Add(customer);

        customer.Slot = Slot;
        customer.customerManager = this;
    }

    CustomerManagerConfig conFig;

    public List<Customer> customers { get; set; } = new List<Customer>();

    public List<Transform> counterTransforms { get; set; }

    public Queue<Customer> customerQueue { get; set; } = new Queue<Customer>();


    public Dictionary<string, GameObject> menuCollection { get; set; }
    public Dictionary<string, GameObject> MenuBoardSlots { get; set; }



    public MenuBoardSlot Slot { get; set; }

    public bool isActive { get; set; }

    Coroutine coroutine;

}
