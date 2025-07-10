using UnityEngine;

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
        EventBus<SetManagementActiveEvent>.Raise(new SetManagementActiveEvent());

        EventBus<FoodSlotHandler>.Raise(new FoodSlotHandler(foodData.foodSprite));

        for (int i = 0; i < foodData.Ingredients.Count; i++)
        {
            EventBus<IngredientsPannelSpawnHandler>.Raise(new IngredientsPannelSpawnHandler(foodData.Ingredients[i].ingredientSprite, foodData.Ingredients[i].qty,0, foodData.displayName));
        }
    }

}
