using System.Collections.Generic;
using TMPro;
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
        // 만약 같은게 있으면 숫자만 올려주고 

        for (int i = 0; i < menuListCollection.Count; i++)
        {
            if (menuListCollection[i].MenuName == menuSpawnHandler.Name)
            {
                string countStr = menuListCollection[i].MenuObj.transform.GetChild((int)MenuInfo.Number).GetComponent<TextMeshProUGUI>().text;
                int count = int.Parse(countStr);
                int count2 = int.Parse(menuSpawnHandler.number);

                count += count2;
                menuListCollection[i].MenuObj.transform.GetChild((int)MenuInfo.Number).GetComponent<TextMeshProUGUI>().text = count.ToString();

                menuListCollection[i].MenuObj.GetComponent<FoodMenuSlot>().FoodAmount = count.ToString();

                return;
            }
        }

        // 아니라면 새로 생성하기 

        GameObject obj = GameObject.Instantiate(conFig.GetGameObjects()[0], menuSpawnHandler.parentTransform);
        obj.transform.GetChild((int)MenuInfo.Image).GetComponent<Image>().sprite = menuSpawnHandler.Image.sprite;
        obj.transform.GetChild((int)MenuInfo.Number).GetComponent<TextMeshProUGUI>().text = menuSpawnHandler.number;

        MenuList menuList = new MenuList();
        menuList.MenuName = menuSpawnHandler.Name;
        menuList.MenuObj = obj;

        obj.GetComponent<FoodMenuSlot>().FoodName = menuSpawnHandler.Name;
        obj.GetComponent<FoodMenuSlot>().FoodAmount = menuSpawnHandler.number;

        menuListCollection.Add(menuList);

    }


    public void AddMenuBoardIndex()
    {
        for (int i = 0; i < MenuBoardSlots.Count; i++)
        {
            if (MenuBoardSlots[i].GetComponent<MenuBoardSlot>().Count > 0)
                MenuIndex.Add(i);
        }
    }

    public void ChoiceRandomMenu(RandomMenuSelectionHandler randomMenuSelectionHandler)
    {
        AddMenuBoardIndex();

        if (MenuIndex.Count <= 0)
        {
            randomMenuSelectionHandler.CustomerManager.Slot = null;
            return;
        }
           

        int randomValue = Random.Range(0, MenuIndex.Count);

        int index = MenuIndex[randomValue];

        MenuBoardSlot slot = MenuBoardSlots[index].GetComponent<MenuBoardSlot>();
        randomMenuSelectionHandler.CustomerManager.Slot = slot;

        int count = slot.Count;
        count -= 1;
        slot.Count = count;

        MenuIndex.Clear();
    }

    public void ReduceMenu(MenuReduceHandler randomMenuSelectionHandler)
    {


        var Slot = randomMenuSelectionHandler.slot;

        int count = int.Parse(Slot.NumberText.text);

        count -= 1;

        Slot.NumberText.text = count.ToString();

        if(count == 0)
        {
            GameObject.Destroy(Slot.gameObject);
            MenuBoardSlots.Remove(Slot.gameObject);
            return;
        }
    }


    public void IsMenuActive(MenuBoardActiveHandler menuBoardActiveHandler)
    {
        MenuBoard.SetActive(menuBoardActiveHandler.isActive);
    }

    public void CreateBoardMenu(MenuBoardSlotSpawnHandler handler)
    {
        // 미리 만들어 둔 프리팹
        GameObject prefab = conFig.GetGameObjects()[1];
        GameObject MenuBoardPrefab = conFig.GetGameObjects()[2];

        foreach (var menuData in menuListCollection)
        {
            var obj = GameObject.Instantiate(prefab, MenuBoard.GetComponent<MenuBoard>().ParentTransform);
            var slot = obj.GetComponent<MenuBoardSlot>();

            // 원본 메뉴 슬롯에서 데이터만 꺼내온다고 가정
            var src = menuData.MenuObj.GetComponent<FoodMenuSlot>();

            Sprite icon = src.transform.GetChild((int)MenuInfo.Image).GetComponent<Image>().sprite;
            string number = src.transform.GetChild((int)MenuInfo.Number).GetComponent<TextMeshProUGUI>().text;
            string name = src.FoodName;

            // 설명 맵에서 찾아서 넘기기
            conFig.GetExplanationData.Map.TryGetValue(name, out string explanation);

            slot.Init(icon, number, name, explanation);

            MenuBoardSlots.Add(obj);
        }
    }
    public void CurrentMenu(MenuLoadedEvent handler)
    {
        handler.CustomerManager.menuListCollection = menuListCollection;
        handler.CustomerManager.MenuBoardSlots = MenuBoardSlots;
    }

    public void DeleteBoardMenu(MenuBoardSlotDeleteHandler handler)
    {
        foreach (var menuData in MenuBoardSlots)
        {
            GameObject.Destroy(menuData);
        }

        MenuBoardSlots.Clear();
    }
    public void DeleteMenuList(FoodMenuDeleteHandler foodMenuDeleteHandler)
    {
        for (int i = 0; i < menuListCollection.Count; i++)
        {
            if (menuListCollection[i].MenuName == foodMenuDeleteHandler.foodname)
            {
                GameObject.Destroy(menuListCollection[i].MenuObj);
                menuListCollection.RemoveAt(i);
                return;
            }
        }
    }

    MenuManagerConfig conFig;

    // 경영ui 일떄
     List<MenuList> menuListCollection = new List<MenuList>();
    // 메뉴판 일떄
     List<GameObject> MenuBoardSlots = new List<GameObject>();

    public List<int> MenuIndex = new List<int>();
    GameObject MenuBoard;

}