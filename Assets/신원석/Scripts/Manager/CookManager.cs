using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.STP;

public enum CookEntityType
{
    Chef,   // 요리사
    Food    // 음식
}
public struct CookInfo
{
    public string foodName;
    public Sprite foodImage;
    public float waitingTime;
}

public class CookManager : baseManager, IGameManager
{
    public CookManager(CookManagerConfig config)
    {
        conFig = config;

        EventBus<CookMakeHandler>.OnEvent += CreateFood;

    }



    ~CookManager()
    {
        EventBus<CookMakeHandler>.OnEvent -= CreateFood;
    }
    public CookManager(BaseScriptableObject baseScriptableObject)
    {
        type = typeof(CookManager);
        conFig = (CookManagerConfig)baseScriptableObject;
    }

    public override void Init()
    {
        GameObject obj = GameObject.Instantiate(conFig.GameObjects[(int)CookEntityType.Chef]);

        Cooker.Add(obj);
    }


    public void CreateFood(CookMakeHandler handler)
    {
        var info = new CookInfo
        {
            foodName = handler.foodname,
            foodImage = handler.Slot.IconImage.sprite,
            waitingTime = 0
        };
        cookInfos.Enqueue(info);
    }

    private void TryStartNextCook()
    {
        if (Cooks.Count > 0 || cookInfos.Count == 0)
        {
            return; // 이미 요리 중이거나 대기 중인 요리가 없음
        }
        // 다음 요리 정보를 가져와서 요리 시작

        var nextCookInfo = cookInfos.Dequeue();
        GameObject obj = GameObject.Instantiate(conFig.GameObjects[(int)CookEntityType.Food]);



        Cook cookComponent = obj.GetComponent<Cook>();
      
        for (int i = 0; i < conFig.Foods.Count; i++)
        {
            if (conFig.Foods[i].displayName == nextCookInfo.foodName)
            {
                cookComponent.WaitingTime = conFig.Foods[i].cookingTime;
                break;
            }
        }
        cookComponent.Setup(nextCookInfo, this);
        Cooks.Enqueue(cookComponent);
    }



    public override void Update()
    {
        TryStartNextCook(); // 새로운 요리가 필요하다면 시작
    }

    CookManagerConfig conFig;

    // 요리중인 목록 한개만 가능함 
    public Queue<Cook> Cooks { get; set; } = new Queue<Cook>();

    // 다음 요리할 내용 미리 등록
    Queue<CookInfo> cookInfos = new Queue<CookInfo>();

    List<GameObject> Cooker = new List<GameObject>();
}
