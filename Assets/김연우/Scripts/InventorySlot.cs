[System.Serializable]
public class InventorySlot
{
    public IngredientData ingredient;
    public int quantity;

    public InventorySlot(IngredientData data)
    {
        ingredient = data;
        quantity = data.qty;  // 스크립터블에 적힌 기본 개수를 초기값으로
    }
}
