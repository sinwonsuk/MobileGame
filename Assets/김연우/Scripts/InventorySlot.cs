using static Unity.Burst.Intrinsics.X86;

[System.Serializable]
public class InventorySlot
{
    public IngredientData ingredient;

    public RunTimeIngredientData runTimeIngredientData;
    public int quantity;

    public InventorySlot(IngredientData data, RunTimeIngredientData runTimeIngredientData)
    {
        ingredient = data;

        runTimeIngredientData.ingredientName = ingredient.ingredientName;

        quantity = runTimeIngredientData.ingredientQty;
    }

    // ��� �̸� 
    // ���� �� �ִ� 



}
