using UnityEngine;

public class DroppableItem : MonoBehaviour
{
    [Header("드랍할 재료")]
    public RunTimeIngredientData ingredientData;

    [Header("드랍 개수")]
    public int amount = 1;

    // 자동으로 ingredientData에서 이름 가져오기
    public string IngredientName => ingredientData != null ? ingredientData.ingredientName : "";
}
