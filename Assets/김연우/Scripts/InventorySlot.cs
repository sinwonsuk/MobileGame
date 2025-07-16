using System;

[Serializable]
public class InventorySlot
{
    public IngredientData ingredient;
    public RunTimeIngredientData runTimeIngredientData;
    public int quantity;

    public InventorySlot(IngredientData data, RunTimeIngredientData runTimeIngredientData)
    {
        ingredient = data;
        this.runTimeIngredientData = runTimeIngredientData;

        this.runTimeIngredientData.ingredientName = ingredient.ingredientName;
        quantity = this.runTimeIngredientData.ingredientQty;
    }
}
