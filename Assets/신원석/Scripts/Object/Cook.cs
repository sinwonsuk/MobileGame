using BackEnd.Quobject.SocketIoClientDotNet.Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class Cook : MonoBehaviour
{

    private float reductionRate = 0f;
    [SerializeField] private float clickIncreasePercent = 0.001f;
    private void OnEnable()
    {
        EventBus<CookMoveHandler>.OnEvent += MoveFood;
        EventBus<CookFillamountHandler>.OnEvent += FillAmount;
        EventBus<CookDeleteHandler>.OnEvent += DeleteFood;
        EventBus<CookTimeReductionEvent>.OnEvent += OnCookTimeReductionEvent;
    }
    private void OnDisable()
    {
        EventBus<CookMoveHandler>.OnEvent -= MoveFood;
        EventBus<CookFillamountHandler>.OnEvent -= FillAmount;
        EventBus<CookDeleteHandler>.OnEvent -= DeleteFood;
        EventBus<CookTimeReductionEvent>.OnEvent -= OnCookTimeReductionEvent;
    }

    void Start()
    {
        canvas = GetComponent<Canvas>();
        button = GetComponent<Button>();
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
        elapsed += Time.deltaTime * (1f + reductionRate);
        foodImage.fillAmount = Mathf.Clamp01(elapsed / WaitingTime);

        if (foodImage.fillAmount >=1.0f && soundCheck==false)
        {
            SoundManager.GetInstance().Sfx_Stop(SoundManager.sfx.Cooking);
            button.enabled = false;
            soundCheck = true;
            TutorialManager.Instance?.TriggerEvent("CookClear");
        }

    }

    public void click()
    {
        // 클릭 시 WaitingTime의 일정 퍼센트만큼 증가
        elapsed += WaitingTime * clickIncreasePercent;

        // fillAmount 즉시 반영
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
            TutorialManager.Instance?.TriggerEvent("TouchOrder");
            StartCoroutine(enumerator(cookMoveHandler));
        }     
    }

    IEnumerator enumerator(CookMoveHandler cookMoveHandler)
    {

        while (true)
        {
            if (foodImage.fillAmount < 1)
            {
                yield return null; 
                continue;
            }

            Vector2 move = new Vector2(cookMoveHandler.TableTransform.position.x, cookMoveHandler.TableTransform.position.y+0.5f);


            if (Vector2.Distance(transform.position, move) < 0.01f)
            {
                EventBus<CustomerStateChangeHandler>.Raise(new CustomerStateChangeHandler(CustomerState.Eat, cookMoveHandler.customer));
                cookMoveHandler.customer.foodPrice = foodPrice;
                cookMoveHandler.customer.foodName = foodName;
               yield break;
            }

            transform.position = Vector3.MoveTowards(transform.position, move, Time.deltaTime * speed);
            yield return null;
        }
    }

    public void DeleteFood(CookDeleteHandler cookDeleteHandler)
    {
        if(customer == cookDeleteHandler.customer)
        Destroy(gameObject);
    }
    private void OnCookTimeReductionEvent(CookTimeReductionEvent evt)
    {
        reductionRate = evt.reductionRate;
    }

    private float speed = 30.0f;
    public string foodName { get; set; } // 음식 이름
    public float WaitingTime { get; set; } // 음식 시간

    public int foodPrice { get; set; } // 음식 가격

    [SerializeField] private Image foodImage; // 음식 이미지

    Button button;
    public Image FoodImage
    {
        get => foodImage;
    }


    private Canvas canvas;

    private CookManager manager; // 요리 매니저

    float elapsed = 0f; // 경과 시간

    bool soundCheck = false; // 사운드 체크   

    Customer customer;
}
