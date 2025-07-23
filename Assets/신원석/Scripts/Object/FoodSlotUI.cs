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
    Name,
    Explanation,
}
public class FoodSlotUI : MonoBehaviour
{

    void OnEnable()
    {
        EventBus<SlotSpawnHandler>.OnEvent += CreateSlot;

    }

    void OnDisable()
    {
        EventBus<SlotSpawnHandler>.OnEvent -= CreateSlot;
       
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


    [SerializeField] Transform slotTransform;
    [SerializeField] Transform menuTransform;
    [SerializeField] GameObject menuSlot;

    public Transform MenuTransform
    {
        get => menuTransform;
    }

}
