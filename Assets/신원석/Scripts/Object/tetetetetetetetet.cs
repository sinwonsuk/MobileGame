using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class tetetetetetetetet : StaffBase
{
    [Header("할당할 StaffStatsSO")]
    public StaffStatsSO stats;              // Inspector 에 할당
    public List<Customer> customers { get; set; } = new List<Customer>();

    public Queue<Cook> Cooks { get; set; } = new Queue<Cook>();

    private bool isWorking = true;        // 현재 일하는 중인가?
    private float timeCounter;              // 남은 시간 카운터
    private void Start()
    {
        EventBus<GetCusomersEvent>.Raise(new GetCusomersEvent(this));
        EventBus<GetFirstCookEvent>.Raise(new GetFirstCookEvent(this));

        timeCounter = stats.timer;
    }



    private void Update()
    {
        // 1) 타이머 계산
        timeCounter -= Time.deltaTime;
        if (timeCounter <= 0f)
        {
            if (isWorking)
            {
                // 일끝 → 휴식
                isWorking = false;
                timeCounter = stats.cooltime;
            }
            else
            {
                // 휴식끝 → 다시 일
                isWorking = true;
                timeCounter = stats.timer;
            }
        }

        // 2) 휴식 중일 때는 Auto-Serving 로직을 건너뛴다
        if (!isWorking)
            return;
        // Debug.Log(Cooks.Count);
        for (int i = 0; i < customers.Count; i++)
        {
            // 큐 자체가 null 이거나 비어있으면 바로 리턴
            if (Cooks == null || Cooks.Count == 0)
                return;


            Customer customer = customers[i];
            if (customer == null)
                continue;

            // Slot 과 NameText 유효성 검사
            MenuBoardSlot slot = customer.Slot;
            if (slot == null || slot.NameText == null)
                continue;

            // customerTable 유효성 검사
            CustomerTable table = customer.customerTable;
            if (table == null)
                continue;

            // FoodClick 컴포넌트 검사 (한 번만 호출)
            FoodClick foodClick = customer.GetComponentInChildren<FoodClick>();
            if (foodClick == null)
                continue;

            // 큐에서 꺼낸 Cook 검사
            Cook cook = Cooks.Peek();
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
