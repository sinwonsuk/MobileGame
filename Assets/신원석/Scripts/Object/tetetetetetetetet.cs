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
        // 큐 자체가 null 이거나 비어있으면 바로 리턴
        if (Cooks == null || Cooks.Count == 0)
            return;

        for (int i = 0; i < customers.Count; i++)
        {
            var customer = customers[i];
            if (customer == null)
                continue;

            // Slot 과 NameText 유효성 검사
            var slot = customer.Slot;
            if (slot == null || slot.NameText == null)
                continue;

            // customerTable 유효성 검사
            var table = customer.customerTable;
            if (table == null)
                continue;

            // FoodClick 컴포넌트 검사 (한 번만 호출)
            var foodClick = customer.GetComponentInChildren<FoodClick>();
            if (foodClick == null)
                continue;

            // 큐에서 꺼낸 Cook 검사
            var cook = Cooks.Peek();
            if (cook == null || cook.FoodImage == null)
                continue;

            // 최종 조건 체크
            if (cook.foodName == slot.NameText.text
                && cook.FoodImage.fillAmount >= 1.0f
                && customer.customerState == CustomerState.Wait
                && !foodClick.Check)
            {
                foodClick.Check = true;
                EventBus<CookMoveHandler>.Raise(
                    new CookMoveHandler(table.transform, customer)
                );
                foodClick.DeleteObject();
            }
        }
    }

}
