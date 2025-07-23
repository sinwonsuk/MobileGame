using TMPro;
using UnityEngine;

public class EnhanceFoodAmountController : MonoBehaviour
{

    private void OnDisable()
    {

    }

    void Start()
    {

    }

    void Update()
    {

    }


    public void FoodAmountConfirmButton()
    {






        FoodData data = enhanceFoodUI.foodData;

        var materials = data.enhanceSteps[data.Level].ingredients;


        for (int i = 0; i < materials.Count; i++)
        {
            if (materials[i].quantity > InventoryManager.Instance.GetItemQty(materials[i].name))
            {
                return;
            }
        }

        for (int i = 0; i < materials.Count; i++)
        {
            EventBus<EnhanceFoodDecreaseHandler>.Raise(new EnhanceFoodDecreaseHandler(data.displayName, materials[i].name, materials[i].quantity));
        }

        data.Level += 1;

        EventBus<SetEnhanceFoodActiveEvent>.Raise(new SetEnhanceFoodActiveEvent());



        //EventBus<UpMenuSpawnHandler>.Raise(new UpMenuSpawnHandler(foodAmountUI.FoodIcon, tempCurrentAmount.ToString(), foodName, MenuParentTransform));
        //EventBus<MenuBoardSlotSpawnHandler>.Raise(new MenuBoardSlotSpawnHandler(tempCurrentAmount.ToString(), foodName));
    }


    [SerializeField] EnhanceFoodUI enhanceFoodUI;
    public Transform MenuParentTransform { get; set; }
}
