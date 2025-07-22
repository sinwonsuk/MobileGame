using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

enum EnhanceFoodClickType
{
    FoodSlot = 0,
    FoodEnhance = 1,
}

public class EnhanceFoodUIManager: baseManager,IGameManager
{
    EnhanceFoodManagerConfig conFig;

    public EnhanceFoodUIManager(EnhanceFoodManagerConfig config)
    {
        conFig = config;
        EventBus<EnhanceFoodUIActiveHandler>.OnEvent += ActiveOn;
        EventBus<SetEnhanceFoodActiveEvent>.OnEvent += ClickFoodImage;
    }
    public EnhanceFoodUIManager(BaseScriptableObject baseScriptableObject)
    {
        type = typeof(UIManager);
        conFig = (EnhanceFoodManagerConfig)baseScriptableObject;
    }

    public void ActiveOn(EnhanceFoodUIActiveHandler enhanceFoodUIActiveHandler)
    {
        if(enhanceFoodUIActiveHandler.isActive == false)
        {
            EnhanceFoodManagerUi[(int)EnhanceFoodClickType.FoodSlot].SetActive(false);
            EnhanceFoodManagerUi[(int)EnhanceFoodClickType.FoodEnhance].SetActive(false);
        }
        else
        {
            EnhanceFoodManagerUi[(int)EnhanceFoodClickType.FoodSlot].SetActive(true);
        }
            
    }


    public override void ActiveOff()
    {
        for (int i = 0; i < EnhanceFoodManagerUi.Count; i++)
        {
            EnhanceFoodManagerUi[i].SetActive(false);
        }
    }

    public override void Init()
    {
        for (int i = 0; i < conFig.EnhanceFoodManagerUi.Count; i++)
        {
            GameObject obj = GameObject.Instantiate(conFig.EnhanceFoodManagerUi[i]);

            EnhanceFoodManagerUi.Add(obj);
        }     
    }


    public void ClickFoodImage(SetEnhanceFoodActiveEvent slotSpawnHandler)
    {
        int foodSlotIdx = (int)EnhanceFoodClickType.FoodSlot;
        int foodAmountIdx = (int)EnhanceFoodClickType.FoodEnhance;

        // FoodSlot의 현재 활성화 상태 읽기
        bool isSlotActive = EnhanceFoodManagerUi[foodSlotIdx].activeSelf;

        // 서로 반대 상태로 토글
        EnhanceFoodManagerUi[foodSlotIdx].SetActive(!isSlotActive);
        EnhanceFoodManagerUi[foodAmountIdx].SetActive(isSlotActive);

    }


    public override void Update()
    {

    }



    List<GameObject> EnhanceFoodManagerUi = new List<GameObject>();
}
