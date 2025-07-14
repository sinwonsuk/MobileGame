using System.Diagnostics;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FoodClick : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        customer = GetComponentInParent<Customer>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // UI 위에 마우스/터치가 있으면 인게임 로직 무시!
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            // 여기서 직접 Raycast로 본인 오브젝트 클릭 여부 체크
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray);
            if (hit && hit.transform == transform)
            {
                // 인게임 오브젝트 터치 처리
                EventBus<CookFillamountHandler>.Raise(new CookFillamountHandler(this));
                EventBus<CookMoveHandler>.Raise(new CookMoveHandler(customer.customerTable.transform, customer));
                Destroy(gameObject);
                // ...이하 로직
            }
        }
    }

    void OnMouseDown()
    {
        //if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        //    return;

        //EventBus<CookFillamountHandler>.Raise(new CookFillamountHandler(this));

        //if (check ==false && customer.customerState == CustomerState.Wait && Image.fillAmount >= 1.0f && customer.Slot.NameText.text == foodName)
        //{
            

        //    EventBus<CookMoveHandler>.Raise(new CookMoveHandler(customer.customerTable.transform,customer));
        //    Destroy(gameObject);
        //}


    }

    public Image Image { get; set; }
    public string foodName { get; set; }

    Customer customer;

    bool check = false;
}
