using UnityEngine;

public class FoodMenuSlot : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DeleteSlot()
    {
        EventBus<FoodIncreaseHandler>.Raise(new FoodIncreaseHandler(FoodName, int.Parse(FoodAmount)));
        EventBus<FoodMenuDeleteHandler>.Raise(new FoodMenuDeleteHandler(FoodName));
    }

    public string FoodName { get; set; }
    public string FoodAmount { get; set; }
}
