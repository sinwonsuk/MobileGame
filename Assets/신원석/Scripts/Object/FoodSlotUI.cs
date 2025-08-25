using BackEnd;
using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UI;

public enum SlotInfo
{
    Name,
    Image,
    Probability,
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

    public void CreateSlot(SlotSpawnHandler h)
    {
        if (h.Slot == null || slotTransform == null) return;

        // 잠금 여부 결정
        bool isLocked = h.foodData != null &&
                        BackendGameData.Instance?.userData != null &&
                        h.foodData.reputation > BackendGameData.Instance.userData.reputation;

        // 인스턴스 생성
        GameObject go = Instantiate(h.Slot, slotTransform);
        go.SetActive(false); // 세팅 중 깜빡임 방지

        var btn = go.GetComponent<Button>();
        var foodSlot = go.GetComponent<FoodSlot>();
        var nameText = go.transform.GetChild((int)SlotInfo.Name).GetComponent<TextMeshProUGUI>();
        var imageUI = go.transform.GetChild((int)SlotInfo.Image).GetComponent<Image>();

        // 리소스 로드 (방어)
        Sprite foodSprite = !string.IsNullOrEmpty(h.Image) ? Resources.Load<Sprite>(h.Image): null;

        if (foodSprite == null)
        {
            Debug.LogWarning($"[FoodSlotUI] Sprite not found at path: {h.Image}");
        }

        if (nameText) nameText.text = h.SlotName;
        if (imageUI) imageUI.sprite = foodSprite;
        if (foodSlot) foodSlot.foodData = h.foodData;

        // 잠금/해제 세팅
        if (btn) btn.interactable = !isLocked;

        if (foodSlot)
        {
            // 잠금 아이콘/텍스트
            foodSlot.RockImage.enabled = isLocked;

            if (foodSlot.Rereputation != null)
                foodSlot.Rereputation.text = isLocked && h.foodData != null
                    ? h.foodData.reputation.ToString()
                    : string.Empty;
        }

        go.SetActive(true);
        slot.Add(go);
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
