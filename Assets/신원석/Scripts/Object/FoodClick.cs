using System.Diagnostics;
using UnityEditor.Tilemaps;
using UnityEngine;
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

    }

    void OnMouseDown()
    {
        EventBus<CookFillamountHandler>.Raise(new CookFillamountHandler(this));

        if (check ==false && customer.customerState == CustomerState.Wait && Image.fillAmount >= 1.0f && customer.Slot.NameText.text == foodName)
        {
            EventBus<CookMoveHandler>.Raise(new CookMoveHandler(customer.customerTable.transform,customer));
            Destroy(gameObject);
        }


    }

    public Image Image { get; set; }
    public string foodName { get; set; }

    Customer customer;

    bool check = false;
}
