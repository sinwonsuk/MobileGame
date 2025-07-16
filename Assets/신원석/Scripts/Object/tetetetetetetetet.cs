using System.Collections.Generic;
using UnityEngine;

public class tetetetetetetetet :MonoBehaviour
{

    public List<Customer> customers { get; set; } = new List<Customer>();

    public Queue<Cook> Cooks { get; set; } = new Queue<Cook>();


    private void Start()
    {
        EventBus<GetCusomersEvent>.Raise(new GetCusomersEvent(this));
        EventBus<GetFirstCookEvent>.Raise(new GetFirstCookEvent(this));
    }



    private void Update()
    {
        for (int i = 0; i < customers.Count; i++)
        {
            if (customers[i].Slot == null || customers[i].customerTable == null || Cooks.Count == 0)
                continue;

            if (Cooks.Peek().foodName == customers[i].Slot.NameText.text && Cooks.Peek().FoodImage.fillAmount >= 1.0f && customers[i].customerState == CustomerState.Wait &&
                customers[i].GetComponentInChildren<FoodClick>().Check ==false)
            {

                customers[i].GetComponentInChildren<FoodClick>().Check = true;

                EventBus<CookMoveHandler>.Raise(new CookMoveHandler(customers[i].customerTable.transform, customers[i]));

                if (customers[i].GetComponentInChildren<FoodClick>() == null)
                {
                    continue;
                }

                customers[i].GetComponentInChildren<FoodClick>().DeleteObject();
               
            }
        }
    }

}
