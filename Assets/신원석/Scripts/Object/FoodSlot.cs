using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.STP;

public class FoodSlot : MonoBehaviour
{
    public FoodData foodData { get; set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateFoodUI()
    {
        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click, false);

        EventBus<SetManagementActiveEvent>.Raise(new SetManagementActiveEvent());

        //.enhanceSteps[conFig.Foods[i].Level - 1].cost;

        EventBus<FoodSlotHandler>.Raise(new FoodSlotHandler(foodData.foodSprite, foodData.displayName, foodData.FoodManual,foodData.enhanceSteps[foodData.Level-1].cost.ToString()));

        for (int i = 0; i < foodData.Ingredients.Count; i++)
        {
            int qty = InventoryManager.Instance.GetItemQty(foodData.Ingredients[i].indate);

            if (qty == -1)
                return;

            EventBus<IngredientsPannelSpawnHandler>.Raise(new IngredientsPannelSpawnHandler(foodData.Ingredients[i].ingredientSprite, qty, 0, foodData.Ingredients[i].ingredientName));
        }
    }

    [SerializeField]
    Image rockImage;

    [SerializeField]
    TextMeshProUGUI rereputation;

    public Image RockImage
    {
        get => rockImage;
        set => rockImage = value;
    }
    public TextMeshProUGUI Rereputation
    {
        get => rereputation;
        set => rereputation = value;
    }

}
