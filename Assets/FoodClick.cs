using System.Diagnostics;
using UnityEngine;

public class FoodClick : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseDown()
    {
        var customer = GetComponentInParent<Customer>();

        if (customer != null && customer.customerState == CustomerState.Wait)
        {
            UnityEngine.Debug.Log("AAAAAA");
        }
            
    }
}
