using BackEnd.Quobject.SocketIoClientDotNet.Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class Cook : MonoBehaviour
{
    


    private void OnEnable()
    {
        EventBus<CookMoveHandler>.OnEvent += MoveFood;
        EventBus<CookFillamountHandler>.OnEvent += FillAmount;
        EventBus<CookFillamountHandler>.OnEvent += FillAmount;
        EventBus<CookDeleteHandler>.OnEvent += DeleteFood;
    }
    private void OnDisable()
    {
        EventBus<CookMoveHandler>.OnEvent -= MoveFood;
        EventBus<CookFillamountHandler>.OnEvent -= FillAmount;
        EventBus<CookDeleteHandler>.OnEvent -= DeleteFood;
    }

    void Start()
    {
        canvas = GetComponent<Canvas>();

        canvas.worldCamera = Camera.main;
    }
    public void Setup(CookInfo info, CookManager mgr)
    {
        manager = mgr;
        foodImage.sprite = info.foodImage;
        foodName = info.foodName;

    }
    // Update is called once per frame
    void Update()
    {
        elapsed += Time.deltaTime;

        foodImage.fillAmount = Mathf.Clamp01(elapsed / WaitingTime);
    }
    public void FillAmount(CookFillamountHandler cookFillamountHandler)
    {
        cookFillamountHandler.FoodClick.Image = foodImage;
        cookFillamountHandler.FoodClick.foodName = foodName;

    }

    public void MoveFood(CookMoveHandler cookMoveHandler)
    {
        if(customer == null)
        {
            manager.Cooks.Clear();
            customer = cookMoveHandler.customer;
            StartCoroutine(enumerator(cookMoveHandler));
        }     
    }

    IEnumerator enumerator(CookMoveHandler cookMoveHandler)
    {

        while (true)
        {
            if (foodImage.fillAmount < 1)
            {
                continue;
            }
            if (Vector2.Distance(transform.position, cookMoveHandler.TableTransform.position) < 0.01f)
            {
                EventBus<CustomerStateChangeHandler>.Raise(new CustomerStateChangeHandler(CustomerState.Eat, cookMoveHandler.customer));

                

                yield break;
            }

            transform.position = Vector3.MoveTowards(transform.position, cookMoveHandler.TableTransform.position, Time.deltaTime * speed);
            yield return null;
        }
    }

    public void DeleteFood(CookDeleteHandler cookDeleteHandler)
    {
        if(customer == cookDeleteHandler.customer)
        Destroy(gameObject);
    }


    private float speed = 30.0f;
    public string foodName { get; set; } // 음식 이름
    public float WaitingTime { get; set; } // 음식 시간

    [SerializeField] private Image foodImage; // 음식 이미지

    private Canvas canvas;

    private CookManager manager; // 요리 매니저

    float elapsed = 0f; // 경과 시간


    Customer customer;
}
