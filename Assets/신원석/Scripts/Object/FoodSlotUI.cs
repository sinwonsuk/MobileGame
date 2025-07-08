using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum SlotInfo
{
    Name,
}
public enum MenuInfo
{
    Image,
    Number,

}
public class FoodSlotUI : MonoBehaviour
{
    struct MenuList
    {
        public string MenuName;
        public GameObject MenuObj;
    }


    void OnEnable()
    {
        EventBus<SlotSpawnHandler>.OnEvent += CreateSlot;
        EventBus<MenuSpawnHandler>.OnEvent += CreateMenu;
        EventBus<FoodMenuDeleteHandler>.OnEvent += DeleteMenuList;
    }

    void OnDisable()
    {
        EventBus<SlotSpawnHandler>.OnEvent -= CreateSlot;
        EventBus<MenuSpawnHandler>.OnEvent -= CreateMenu;
        EventBus<FoodMenuDeleteHandler>.OnEvent -= DeleteMenuList;
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateSlot(SlotSpawnHandler slotSpawnHandler)
    {
        GameObject obj = Instantiate(slotSpawnHandler.Slot, slotTransform);
        Sprite foodSprite = Resources.Load<Sprite>(slotSpawnHandler.Image);
        obj.GetComponent<Image>().sprite = foodSprite;
        obj.transform.GetChild((int)SlotInfo.Name).GetComponent<TextMeshProUGUI>().text = slotSpawnHandler.SlotName;
        obj.GetComponent<FoodSlot>().foodData = slotSpawnHandler.foodData;
    }

    public void CreateMenu(MenuSpawnHandler menuSpawnHandler)
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

        GameObject obj = Instantiate(menuSlot, menuTransform);
        obj.transform.GetChild((int)MenuInfo.Image).GetComponent<Image>().sprite = menuSpawnHandler.Image.sprite;
        obj.transform.GetChild((int)MenuInfo.Number).GetComponent<TextMeshProUGUI>().text = menuSpawnHandler.number;

        MenuList menuList = new MenuList();
        menuList.MenuName = menuSpawnHandler.Name;
        menuList.MenuObj = obj;


        obj.GetComponent<FoodMenuSlot>().FoodName = menuSpawnHandler.Name;
        obj.GetComponent<FoodMenuSlot>().FoodAmount = menuSpawnHandler.number;

        menuListCollection.Add(menuList);

    }

    public void DeleteMenuList(FoodMenuDeleteHandler foodMenuDeleteHandler)
    {
        for (int i = 0; i < menuListCollection.Count; i++)
        {
            if (menuListCollection[i].MenuName == foodMenuDeleteHandler.foodname)
            {
                Destroy(menuListCollection[i].MenuObj);
                menuListCollection.RemoveAt(i);
                return;
            }
        }        
    }



    List<MenuList> menuListCollection = new List<MenuList>();

    [SerializeField] Transform slotTransform;
    [SerializeField] Transform menuTransform;
    [SerializeField] GameObject menuSlot;
}
