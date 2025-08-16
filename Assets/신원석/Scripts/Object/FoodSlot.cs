using UnityEngine;
using UnityEngine.UI;

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

        EventBus<FoodSlotHandler>.Raise(new FoodSlotHandler(foodData.foodSprite,foodData.displayName,foodData.FoodManual));

        for (int i = 0; i < foodData.Ingredients.Count; i++)
        {
            int qty = InventoryManager.Instance.GetItemQty(foodData.Ingredients[i].indate);

            if (qty == -1)
                return;

            EventBus<IngredientsPannelSpawnHandler>.Raise(new IngredientsPannelSpawnHandler(foodData.Ingredients[i].ingredientSprite, qty, 0, foodData.Ingredients[i].ingredientName));
        }
    }

    public void buttonOnOff(bool check)
    {
        myButton.interactable = check;
    }


    [SerializeField] private Button myButton;
}
