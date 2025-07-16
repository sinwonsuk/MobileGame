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

            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            EventBus<CookFillamountHandler>.Raise(new CookFillamountHandler(this));

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

            if (hit && hit.transform == transform && Check == false && customer.customerState == CustomerState.Wait && Image.fillAmount >= 1.0f && customer.Slot.NameText.text == foodName)
            {
                //EventBus<CookFillamountHandler>.Raise(new CookFillamountHandler(this));
                EventBus<CookMoveHandler>.Raise(new CookMoveHandler(customer.customerTable.transform, customer));
                Destroy(gameObject);
                // ...이하 로직
            }
        }
    }

    public void DeleteObject()
    {
        Destroy(gameObject);
    }

    public Image Image { get; set; }
    public string foodName { get; set; }

    Customer customer;

    public bool Check { get; set; } = false;
}
