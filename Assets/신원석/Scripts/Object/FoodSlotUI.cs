using BackEnd;
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
    Image,
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
        EventBus<FoodSlotDeleteHandler>.OnEvent += DeleteSlot;

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
        if(slotSpawnHandler.foodData.reputation > BackendGameData.Instance.userData.reputation)
        {
            GameObject obj = Instantiate(slotSpawnHandler.Slot, slotTransform);
            obj.SetActive(false);
            obj.GetComponent<Button>().interactable = false;
            Sprite foodSprite = Resources.Load<Sprite>(slotSpawnHandler.Image);
            obj.transform.GetChild((int)SlotInfo.Name).GetComponent<TextMeshProUGUI>().text = slotSpawnHandler.SlotName;
            obj.transform.GetChild((int)SlotInfo.Image).GetComponent<Image>().sprite = foodSprite;
            obj.GetComponent<FoodSlot>().foodData = slotSpawnHandler.foodData;
            obj.GetComponent<FoodSlot>().RockImage.enabled = true;
            obj.GetComponent<FoodSlot>().Rereputation.text = slotSpawnHandler.foodData.reputation.ToString();
            obj.SetActive(true);
            slot.Add(obj);
        }
        else
        {
            GameObject obj = Instantiate(slotSpawnHandler.Slot, slotTransform);
            obj.GetComponent<Button>().interactable = true;
            Sprite foodSprite = Resources.Load<Sprite>(slotSpawnHandler.Image);
            obj.transform.GetChild((int)SlotInfo.Name).GetComponent<TextMeshProUGUI>().text = slotSpawnHandler.SlotName;
            obj.transform.GetChild((int)SlotInfo.Image).GetComponent<Image>().sprite = foodSprite;
            obj.GetComponent<FoodSlot>().foodData = slotSpawnHandler.foodData;
            obj.GetComponent<FoodSlot>().RockImage.enabled = false;
            slot.Add(obj);
        }



    }

    public void DeleteSlot(FoodSlotDeleteHandler slotSpawnHandler)
    {
        for (int i = 0; i < slot.Count; i++)
        {
            Destroy(slot[i]);
        }
        slot.Clear();
    }


    [SerializeField] Transform slotTransform;
    [SerializeField] Transform menuTransform;
    [SerializeField] GameObject menuSlot;

    public Transform MenuTransform
    {
        get => menuTransform;
    }
    List<GameObject> slot = new List<GameObject>();
}
