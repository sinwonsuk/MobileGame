using System.Diagnostics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
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
        if (Mouse.current == null)
        {
            UnityEngine.Debug.LogWarning("Mouse.current is NULL!");
            return;
        }

        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {

            EventBus<CookFillamountHandler>.Raise(new CookFillamountHandler(this));


            Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(touchPosition);

            RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

            if (hit && hit.transform == transform && Check == false && customer.customerState == CustomerState.Wait && Image.fillAmount >= 1.0f && customer.Slot.NameText.text == foodName)
            {
                SoundManager.GetInstance().SfxPlay(SoundManager.sfx.FoodMove, false);
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
