using UnityEngine;

public struct IngredientsPannelSpawnHandler : IEvent
{
    public IngredientsPannelSpawnHandler(string Image,int CurrentIngredientAmount, int NeedIngredientAmount,string IngredientName)
    {
        this.Image = Image;
        this.CurrentIngredientAmount = CurrentIngredientAmount;
        this.NeedIngredientAmount = NeedIngredientAmount;
        this.IngredientName = IngredientName;
    }

    public string Image { get; set; }

    public string IngredientName { get; set; }

    public int NeedIngredientAmount { get; set; }

    public int CurrentIngredientAmount { get; set; }

}
