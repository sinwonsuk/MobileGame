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
    }

    public CustomerManager(BaseScriptableObject baseScriptableObject)
    {
        type = typeof(CustomerManager);
        conFig = (CustomerManagerConfig)baseScriptableObject;
    }

    public override void Init()
    {
        EventBus<MenuLoadedEvent>.Raise(new MenuLoadedEvent(this));
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
                    //EventBus<MenuReduceHandler>.Raise(new MenuReduceHandler(Slot));
                }

                yield return null;
            }
        }
    }




    public void CheckMenu()
    {
        EventBus<RandomMenuSelectionHandler>.Raise(new RandomMenuSelectionHandler(this));

        if (Slot == null)
            return;

        GameObject obj = GameObject.Instantiate(conFig.GetGameObjects()[0]);
        obj.GetComponent<Customer>().Slot = Slot;
    }

    CustomerManagerConfig conFig;

    List<Customer> customers = new List<Customer>();
    public Dictionary<string, GameObject> menuCollection { get; set; }
    public Dictionary<string, GameObject> MenuBoardSlots { get; set; }

    public MenuBoardSlot Slot { get; set; }

    public bool isActive { get; set; }

    Coroutine coroutine;

}
