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
        // 음식 삭제하면 수량 늘려야 하니까 한거구나 
        EventBus<FoodIncreaseHandler>.Raise(new FoodIncreaseHandler(FoodName, int.Parse(FoodAmount)));

        EventBus<FoodMenuDeleteHandler>.Raise(new FoodMenuDeleteHandler(FoodName));
       // EventBus<FoodMenuDeleteHandler>.Raise(new FoodMenuDeleteHandler(FoodName));
    }

    public string FoodName { get; set; }
    public string FoodAmount { get; set; }
}
