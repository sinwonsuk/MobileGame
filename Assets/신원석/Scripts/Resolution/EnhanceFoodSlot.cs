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



        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click, false);

        if (foodData.Level == foodData.enhanceSteps.Count)
            return;


        EventBus<SetEnhanceFoodActiveEvent>.Raise(new SetEnhanceFoodActiveEvent());
        EventBus<EnhanceFoodSlotHandler>.Raise(new EnhanceFoodSlotHandler(foodData,foodData.foodSprite, foodData.displayName, foodData.Level, foodData.enhanceSteps[foodData.Level-1].cost, foodData.enhanceSteps[foodData.Level].step, foodData.enhanceSteps[foodData.Level].cost, foodData.enhanceSteps.Count));

        for (int i = 0; i < foodData.Ingredients.Count; i++)
        {
            int qty = InventoryManager.Instance.GetItemQty(foodData.Ingredients[i].indate);

            if (qty == -1)
                return;

            int futureQty = 0;


            var ingredients = foodData.enhanceSteps[foodData.Level].ingredients;


            for (int j = 0; j < ingredients.Count; j++)
            {
                if (ingredients[j].indate == foodData.Ingredients[i].indate)
                {
                    futureQty = ingredients[j].quantity;
                    break;
                }
            }

            EventBus<IngredientsPannelSpawnHandler>.Raise(new IngredientsPannelSpawnHandler(foodData.Ingredients[i].ingredientSprite, qty, futureQty, foodData.Ingredients[i].ingredientName));
        }
    }

    public FoodData foodData { get; set; }
}
