using UnityEngine;
using UnityEngine.UI;

public class EnhanceFoodSlot : MonoBehaviour
{
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
        if (foodData.Level == foodData.enhanceSteps.Count-1)
            return;


        EventBus<SetEnhanceFoodActiveEvent>.Raise(new SetEnhanceFoodActiveEvent());

        // 아직 가격 미정 
        EventBus<EnhanceFoodSlotHandler>.Raise(new EnhanceFoodSlotHandler(foodData,foodData.foodSprite, foodData.displayName, foodData.Level+1, foodData.enhanceSteps[foodData.Level].cost, foodData.enhanceSteps[foodData.Level+1].step, foodData.enhanceSteps[foodData.Level + 1].cost, foodData.enhanceSteps.Count));

        for (int i = 0; i < foodData.Ingredients.Count; i++)
        {
            int qty = InventoryManager.Instance.GetItemQty(foodData.Ingredients[i].ingredientName);

            if (qty == -1)
                return;

            EventBus<IngredientsPannelSpawnHandler>.Raise(new IngredientsPannelSpawnHandler(foodData.Ingredients[i].ingredientSprite, qty,foodData.enhanceSteps[foodData.Level].ingredients[i].quantity, foodData.Ingredients[i].ingredientName));
        }


    }

    public FoodData foodData { get; set; }
}
