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
        // �ϴ� on off �ϰ� 
        EventBus<SetManagementActiveEvent>.Raise(new SetManagementActiveEvent());

        // ���ĵ� current�� �پ���� �� �׷� ���� �����͸� �ǵ���� �ϴµ� 




    }
}
