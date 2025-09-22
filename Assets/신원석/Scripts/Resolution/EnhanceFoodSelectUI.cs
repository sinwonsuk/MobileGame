using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class EnhanceFoodSelectUI : MonoBehaviour
{

    void OnEnable()
    {
        EventBus<EnhanceFoodSlotSpawnHandler>.OnEvent += CreateSlot;
        EventBus<EnhanceFoodSlotDeleteHandler>.OnEvent += DeleteSlot;
    }

    void OnDisable()
    {
        EventBus<EnhanceFoodSlotSpawnHandler>.OnEvent -= CreateSlot;
        EventBus<EnhanceFoodSlotDeleteHandler>.OnEvent -= DeleteSlot;

    }
    void Start()
    {
  
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CreateSlot(EnhanceFoodSlotSpawnHandler slotSpawnHandler)
    {
        if (slotSpawnHandler.foodData.reputation > BackendGameData.Instance.userData.reputation)
            return;


        GameObject obj = Instantiate(slotSpawnHandler.Slot, slotTransform);
        Sprite foodSprite = Resources.Load<Sprite>(slotSpawnHandler.Image);
        obj.transform.GetChild((int)SlotInfo.Name).GetComponent<TextMeshProUGUI>().text = slotSpawnHandler.SlotName;
        obj.transform.GetChild((int)SlotInfo.Image).GetComponent<Image>().sprite = foodSprite;
        obj.transform.GetChild((int)SlotInfo.Probability).GetComponent<TextMeshProUGUI>().text = slotSpawnHandler.foodData.enhanceSteps[slotSpawnHandler.foodData.Level-1].successRate.ToString();
        obj.GetComponent<EnhanceFoodSlot>().foodData = slotSpawnHandler.foodData;

        slot.Add(obj);
    }

    public void DeleteSlot(EnhanceFoodSlotDeleteHandler slotSpawnHandler)
    {
        for (int i = 0; i < slot.Count; i++)
        {
            Destroy(slot[i]);
        }
        slot.Clear();
    }

    [SerializeField] Transform slotTransform;
    [SerializeField] GameObject menuSlot;


    List<GameObject> slot = new List<GameObject>();

}
