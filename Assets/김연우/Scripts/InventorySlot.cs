[System.Serializable]
public class InventorySlot
{
    public IngredientData ingredient;
    public int quantity;

    public InventorySlot(IngredientData data)
    {
        ingredient = data;
        quantity = data.qty; 
    }
}
