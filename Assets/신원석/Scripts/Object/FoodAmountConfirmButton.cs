using UnityEngine;

public class FoodAmountConfirmButton : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void check()
    {
        // 일단 on off 하고 
        EventBus<SetManagementActiveEvent>.Raise(new SetManagementActiveEvent());

        // 음식들 current도 줄어들어야 함 그럼 음식 데이터를 건드려야 하는데 




    }
}
