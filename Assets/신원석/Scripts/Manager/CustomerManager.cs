using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.Rendering.DebugUI;
using static UnityEngine.RuleTile.TilingRuleOutput;
using Transform = UnityEngine.Transform;

public enum CustomerType
{
    FruitCustomer,
    WhiteCatCustomer,
    BlackCatCustomer,
}

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

    public override void Destory()
    {
        EventBus<CustomerSpawnHandler>.OnEvent -= SpawnCustomer;
        EventBus<GetCusomersEvent>.OnEvent -= GetCustomerEvent;
    }

    public override void Init()
    {
        EventBus<MenuLoadedEvent>.Raise(new MenuLoadedEvent(this));


        counterTransforms = GameObject.FindWithTag("Counter").GetComponent<OrderCounter>().QueuePositions;
        waitingCustomerTransform = GameObject.FindWithTag("WaitingCustomer").transform;
    }

    public void GetCustomerEvent(GetCusomersEvent getCustomersEvent)
    {
        getCustomersEvent.employee.customers = customers;
    }


    public void SpawnCustomer(CustomerSpawnHandler customerSpawnHandler)
    {
        for (int i = 0; i < conFig.Customers.Count; i++)
        {
            GameObject.Instantiate(conFig.Customers[i]);
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

    public void UpdateQueueDestinations()
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
        List<string> availableMenus = new List<string>();

        foreach (var kvp in MenuBoardSlots)
        {
            MenuBoardSlot slots = kvp.Value.GetComponent<MenuBoardSlot>();
            if (slots.Count > 0)
            {
                availableMenus.Add(kvp.Key);
            }
        }

        if (availableMenus.Count == 0)
            return;


        // 손님 후보 인덱스로 저장 
        List<int> candidateIndices = new List<int>();

        // 음식 타입인 손님 전부 대려옴 
        for (int i = 0; i < conFig.Customers.Count; i++)
        {
            Customer prefab = conFig.Customers[i].GetComponent<Customer>();
            foreach (var food in prefab.FoodDatas)
            {
                if (availableMenus.Contains(food.displayName))
                {
                    candidateIndices.Add(i);
                    break;
                }
            }
        }

        // 손님이 원하는 음식이 아니면 리턴 
        if (candidateIndices.Count == 0)
            return;

        // 손님 중 랜덤 선택
        int candidateIndex = candidateIndices[Random.Range(0, candidateIndices.Count)];
        GameObject prefabObj = conFig.Customers[candidateIndex];
        Customer prefabCust = prefabObj.GetComponent<Customer>();
        // 이제 손님 나옴 


        // 이제 이 손님이 원하는 음식 넣어줌(손님이 원하는 음식이 여러개 있을경우)
        List<string> matchedMenus = new List<string>();
        foreach (var food in prefabCust.FoodDatas)
        {
            if (availableMenus.Contains(food.displayName))
                matchedMenus.Add(food.displayName);
        }

        // 손님이 원하는 음식중의 랜덤으로 선택 
        string chosenMenu = matchedMenus[Random.Range(0, matchedMenus.Count)];
        GameObject menuObj = MenuBoardSlots[chosenMenu];
        MenuBoardSlot slot = menuObj.GetComponent<MenuBoardSlot>();

        // 손님 init 위치설정 
        float postX = Random.Range(spawnMinX, spawnMaxX);
        float postY = Random.Range(spawnMinY, spawnMaxY);
        Vector3 vector = new Vector3(waitingCustomerTransform.position.x + postX, waitingCustomerTransform.position.y + postY);

        GameObject obj = GameObject.Instantiate(prefabObj, vector, Quaternion.identity);
        Customer customerObj = obj.GetComponent<Customer>();

        // 매니저의 손님 저장 
        customers.Add(customerObj);
        // 손님 init 설정 
        customerObj.Slot = slot;
        customerObj.customerManager = this;

        slot.Count--;
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

    private Transform waitingCustomerTransform;

    private float spawnMinX = -2.0f;
    private float spawnMaxX = 5.0f;
    private float spawnMinY = -0.2f;
    private float spawnMaxY = 0.5f;
}
