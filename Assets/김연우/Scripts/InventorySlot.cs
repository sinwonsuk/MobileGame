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
        this.runTimeIngredientData.indate = ingredient.indate;
        quantity = this.runTimeIngredientData.ingredientQty;
    }
}
