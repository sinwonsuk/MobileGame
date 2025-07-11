using System.Collections.Generic;
using TMPro;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public struct MenuList
{
    public string MenuName;
    public GameObject MenuObj;
}
public class MenuManager : baseManager, IGameManager
{
   

    public MenuManager(MenuManagerConfig config)
    {
        conFig = config;
        EventBus<UpMenuSpawnHandler>.OnEvent += CreateMenu;
        EventBus<FoodMenuDeleteHandler>.OnEvent += DeleteMenuList;
        EventBus<MenuBoardSlotSpawnHandler>.OnEvent += CreateBoardMenu;
        EventBus<MenuBoardSlotDeleteHandler>.OnEvent += DeleteBoardMenu;
        EventBus<MenuBoardActiveHandler>.OnEvent += IsMenuActive;
        EventBus<RandomMenuSelectionHandler>.OnEvent += ChoiceRandomMenu;
        EventBus<MenuReduceHandler>.OnEvent += ReduceMenu;
        EventBus<MenuLoadedEvent>.OnEvent += CurrentMenu;
    }
    ~MenuManager()
    {
        EventBus<UpMenuSpawnHandler>.OnEvent -= CreateMenu;
        EventBus<FoodMenuDeleteHandler>.OnEvent -= DeleteMenuList;
        EventBus<MenuBoardSlotSpawnHandler>.OnEvent -= CreateBoardMenu;
        EventBus<MenuBoardSlotDeleteHandler>.OnEvent -= DeleteBoardMenu;
        EventBus<MenuBoardActiveHandler>.OnEvent -= IsMenuActive;
        EventBus<RandomMenuSelectionHandler>.OnEvent -= ChoiceRandomMenu;
        EventBus<MenuReduceHandler>.OnEvent -= ReduceMenu;
        EventBus<MenuLoadedEvent>.OnEvent -= CurrentMenu;
    }

    public MenuManager(BaseScriptableObject baseScriptableObject)
    {
        type = typeof(MenuManager);
        conFig = (MenuManagerConfig)baseScriptableObject;
    }

    public override void Init()
    {
       GameObject obj =GameObject.Instantiate(conFig.GetGameObjects()[2]);
        MenuBoard = obj;
    }

    public override void ActiveOff()
    {
        MenuBoard.SetActive(false);
    }

    public override void Update()
    {

    }

    public void CreateMenu(UpMenuSpawnHandler menuSpawnHandler)
    {
        // ���� ������ ������ ���ڸ� �÷��ְ� 
        if (menuCollection.TryGetValue(menuSpawnHandler.Name, out var menu))
        {
            string countStr = menu.transform.GetChild((int)MenuInfo.Number).GetComponent<TextMeshProUGUI>().text;

            int count = int.Parse(countStr);
            int count2 = int.Parse(menuSpawnHandler.number);
            count += count2;

            menu.transform.GetChild((int)MenuInfo.Number).GetComponent<TextMeshProUGUI>().text = count.ToString();
            menu.GetComponent<FoodMenuSlot>().FoodAmount = count.ToString();

            return;
        }


        // �ƴ϶�� ���� �����ϱ� 

        GameObject obj = GameObject.Instantiate(conFig.GetGameObjects()[0], menuSpawnHandler.parentTransform);
        obj.transform.GetChild((int)MenuInfo.Image).GetComponent<Image>().sprite = menuSpawnHandler.Image.sprite;
        obj.transform.GetChild((int)MenuInfo.Number).GetComponent<TextMeshProUGUI>().text = menuSpawnHandler.number;



        obj.GetComponent<FoodMenuSlot>().FoodName = menuSpawnHandler.Name;
        obj.GetComponent<FoodMenuSlot>().FoodAmount = menuSpawnHandler.number;

        menuCollection.Add(menuSpawnHandler.Name, obj);
    }


    public void AddMenuBoardIndex()
    {
        MenuIndex.Clear();

        foreach (var kvp in MenuBoardSlots)
        {
            var slotObj = kvp.Value;
            var slot = slotObj.GetComponent<MenuBoardSlot>();
            if (slot.Count > 0)
                MenuIndex.Add(kvp.Key);
        }
    }

    public void ChoiceRandomMenu(RandomMenuSelectionHandler handler)
    {
        AddMenuBoardIndex();

        // ������ ������ ������ null
        if (MenuIndex.Count == 0)
        {
            handler.CustomerManager.Slot = null;
            return;
        }

        // �������� Ű �ϳ� �̱�
        int rnd = Random.Range(0, MenuIndex.Count);
        string chosenKey = MenuIndex[rnd];
        GameObject obj = MenuBoardSlots[chosenKey];
        MenuBoardSlot slot = obj.GetComponent<MenuBoardSlot>();

        // ���õ� ������ �� �Ŵ����� ����
        handler.CustomerManager.Slot = slot;


        slot.Count--;

        MenuIndex.Clear();
    }

    public void ReduceMenu(MenuReduceHandler randomMenuSelectionHandler)
    {


        var Slot = randomMenuSelectionHandler.slot;

        int count = int.Parse(Slot.NumberText.text);

        count -= 1;

        Slot.NumberText.text = count.ToString();


        if (menuCollection.TryGetValue(Slot.NameText.text, out var menu))
        {
            menu.transform.GetChild((int)MenuInfo.Number).GetComponent<TextMeshProUGUI>().text = count.ToString();
        }

        if (count == 0)
        {

            MenuBoardSlots.Remove(Slot.NameText.text);
            menuCollection.Remove(Slot.NameText.text);

            GameObject.Destroy(menu.gameObject);
            GameObject.Destroy(Slot.gameObject);

            return;
        }
    }


    public void IsMenuActive(MenuBoardActiveHandler menuBoardActiveHandler)
    {
        MenuBoard.SetActive(menuBoardActiveHandler.isActive);
    }

    public void CreateBoardMenu(MenuBoardSlotSpawnHandler handler)
    {
        // �̸� ����� �� ������
        GameObject prefab = conFig.GetGameObjects()[1];
        GameObject MenuBoardPrefab = conFig.GetGameObjects()[2];


        // ������ ���ڸ� ������ 

        if (MenuBoardSlots.TryGetValue(handler.Name, out var menu))
        {
            MenuBoardSlot BoardSlot = menu.GetComponent<MenuBoardSlot>();

            string countStr = BoardSlot.NumberText.text;
            int count = int.Parse(countStr);
            int count2 = int.Parse(handler.number);

            count += count2;

            BoardSlot.Count += count2;

            BoardSlot.NumberText.text = count.ToString();
            
            return;
        }


        var obj = GameObject.Instantiate(prefab, MenuBoard.GetComponent<MenuBoard>().ParentTransform);
        var slot = obj.GetComponent<MenuBoardSlot>();

        // ���� �޴� ���Կ��� �����͸� �����´ٰ� ����
        var src = menuCollection[handler.Name].GetComponent<FoodMenuSlot>();

        Sprite icon = src.transform.GetChild((int)MenuInfo.Image).GetComponent<Image>().sprite;
        string number = src.transform.GetChild((int)MenuInfo.Number).GetComponent<TextMeshProUGUI>().text;
        string name = src.FoodName;

        // ���� �ʿ��� ã�Ƽ� �ѱ��
        conFig.GetExplanationData.Map.TryGetValue(name, out string explanation);

        slot.Init(icon, number, name, explanation);

        MenuBoardSlots.Add(handler.Name,obj);

    }
    public void CurrentMenu(MenuLoadedEvent handler)
    {
        handler.CustomerManager.menuCollection = menuCollection;
        handler.CustomerManager.MenuBoardSlots = MenuBoardSlots;
    }

    public void DeleteBoardMenu(MenuBoardSlotDeleteHandler handler)
    {
        foreach (var menuData in MenuBoardSlots.Values)
        {
            GameObject.Destroy(menuData);
        }

        MenuBoardSlots.Clear();
    }
    public void DeleteMenuList(FoodMenuDeleteHandler foodMenuDeleteHandler)
    {
        string name = foodMenuDeleteHandler.foodname;

        // 1) �濵 UI �޴� ����
        if (menuCollection.TryGetValue(name, out GameObject menuObj))
        {
            GameObject.Destroy(menuObj);
            menuCollection.Remove(name);
        }

        // 2) �޴��� UI ���� ����
        if (MenuBoardSlots.TryGetValue(name, out GameObject boardObj))
        {
            GameObject.Destroy(boardObj);
            MenuBoardSlots.Remove(name);
        }

    }

    MenuManagerConfig conFig;

    // �濵ui �ϋ�
    Dictionary<string, GameObject> menuCollection = new Dictionary<string, GameObject>();

    // �޴��� �ϋ� 
    Dictionary<string, GameObject> MenuBoardSlots = new Dictionary<string, GameObject>();


    public List<string> MenuIndex = new List<string>();
    GameObject MenuBoard;

}