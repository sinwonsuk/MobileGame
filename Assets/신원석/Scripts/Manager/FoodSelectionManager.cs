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
        EventBus<ManagementActiveHandler>.OnEvent += IsManagementActive;
        EventBus<SetManagementActiveEvent>.OnEvent += ClickFoodImage;
        EventBus<SetMenuParentTransformHandler>.OnEvent += GetMenuParentTransform;
    }
    ~FoodSelectionManager()
    {
        EventBus<ManagementActiveHandler>.OnEvent -= IsManagementActive;
        EventBus<SetManagementActiveEvent>.OnEvent -= ClickFoodImage;
        EventBus<SetMenuParentTransformHandler>.OnEvent -= GetMenuParentTransform;
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
    // 아래 버튼 
    public void IsManagementActive(ManagementActiveHandler slotSpawnHandler)
    {
        slotUI[(int)slotSpawnHandler.clickType].SetActive(slotSpawnHandler.isActive);
    }

    public override void Update()
    {

    }

    public void GetMenuParentTransform(SetMenuParentTransformHandler setMenuParentTransformHandler)
    {
        setMenuParentTransformHandler.Controller.MenuParentTransform = slotUI[(int)ClickType.FoodSlot].GetComponent<FoodSlotUI>().MenuTransform;
    }


    public void ClickFoodImage(SetManagementActiveEvent slotSpawnHandler)
    {

        int foodSlotIdx = (int)ClickType.FoodSlot;
        int foodAmountIdx = (int)ClickType.FoodAmount;

        // FoodSlot의 현재 활성화 상태 읽기
        bool isSlotActive = slotUI[foodSlotIdx].activeSelf;

        // 서로 반대 상태로 토글
        slotUI[foodSlotIdx].SetActive(!isSlotActive);
        slotUI[foodAmountIdx].SetActive(isSlotActive);

        //slotUI[(int)ClickType.FoodSlot].SetActive(false);
        //slotUI[(int)ClickType.FoodAmount].SetActive(true);
    }

    FoodSelectionManagerConfig conFig;

    List<GameObject> slotUI;

}
