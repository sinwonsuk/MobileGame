using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.RuleTile.TilingRuleOutput;
using Transform = UnityEngine.Transform;

public enum ClickType
{
    FoodSlot,
    FoodAmount,
}

public class FoodSelectionManager : baseManager, IGameManager
{
    public FoodSelectionManager(FoodSelectionManagerConfig config)
    {
        conFig = config;
        slotUI = new List<GameObject>();
        EventBus<ManagementActiveHandler>.OnEvent += IsActive;
        EventBus<SetManagementActiveEvent>.OnEvent += ClickFoodImage;
    }
    ~FoodSelectionManager()
    {
        EventBus<ManagementActiveHandler>.OnEvent -= IsActive;
        EventBus<SetManagementActiveEvent>.OnEvent -= ClickFoodImage;
    }

    public FoodSelectionManager(BaseScriptableObject baseScriptableObject)
    {
        type = typeof(FoodSelectionManager);
        conFig = (FoodSelectionManagerConfig)baseScriptableObject;
    }

    public override void Init()
    {
        for (int i = 0; i < conFig.GetGameObjects().Count; i++)
        {
            GameObject obj = GameObject.Instantiate(conFig.GetGameObjects()[i]);
            slotUI.Add(obj);
        }
    }

    public override void ActiveOff()
    {
        for (int i = 0; i < slotUI.Count; i++)
        {
            slotUI[i].SetActive(false);
        }
    }
    // �Ʒ� ��ư 
    public void IsActive(ManagementActiveHandler slotSpawnHandler)
    {
        if(slotUI[(int)ClickType.FoodSlot].activeSelf ==true)
            slotUI[(int)ClickType.FoodAmount].SetActive(slotSpawnHandler.isActive);

        slotUI[(int)ClickType.FoodSlot].SetActive(slotSpawnHandler.isActive);
    }

    public override void Update()
    {
       
    }

    public void ClickFoodImage(SetManagementActiveEvent slotSpawnHandler)
    {

        int foodSlotIdx = (int)ClickType.FoodSlot;
        int foodAmountIdx = (int)ClickType.FoodAmount;

        // FoodSlot�� ���� Ȱ��ȭ ���� �б�
        bool isSlotActive = slotUI[foodSlotIdx].activeSelf;

        // ���� �ݴ� ���·� ���
        slotUI[foodSlotIdx].SetActive(!isSlotActive);
        slotUI[foodAmountIdx].SetActive(isSlotActive);

        //slotUI[(int)ClickType.FoodSlot].SetActive(false);
        //slotUI[(int)ClickType.FoodAmount].SetActive(true);
    }

    FoodSelectionManagerConfig conFig;

    List<GameObject> slotUI;

}
