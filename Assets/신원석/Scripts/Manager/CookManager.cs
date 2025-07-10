using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.STP;

public enum CookEntityType
{
    Chef,   // �丮��
    Food    // ����
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

    //public void CreateFood(CookMakeHandler cookMakeHandler)
    //{
    //    //CookInfo cookInfo = new CookInfo
    //    //{
    //    //    foodName = cookMakeHandler.foodname,
    //    //    foodImage = cookMakeHandler.Slot.IconImage.sprite,
    //    //    waitingTime = 0f // �ʱ� ��� �ð��� 0���� ����
    //    //};

    //    cookQueue.Enqueue(cookInfo);

    //    if( cooks.Count > 0)
    //    {
    //        return;
    //    }
    //    else
    //    {
    //        // �丮���� ����� ������ �丮 ����
    //        GameObject obj = GameObject.Instantiate(conFig.GameObjects[(int)CookEntityType.Food], conFig.GameObjects[(int)CookEntityType.Chef].transform);

    //        // � �����̳� , �̸��� ���� 
    //        obj.GetComponent<Cook>().foodName = cookMakeHandler.foodname;
    //        obj.GetComponent<Cook>().FoodImage.sprite = cookMakeHandler.Slot.IconImage.sprite;
    //        obj.GetComponent<Cook>().Cooks = cooks;


    //        // ������ ��� �ð� ����
    //        for (int i = 0; i < conFig.Foods.Count; i++)
    //        {
    //            if (conFig.Foods[i].displayName == cookMakeHandler.foodname)
    //            {
    //                obj.GetComponent<Cook>().WaitingTime = conFig.Foods[i].waitingTime;
    //                break;
    //            }
    //        }

    //        cooks.Enqueue(obj.GetComponent<Cook>());
    //    }



    //}
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
            return; // �̹� �丮 ���̰ų� ��� ���� �丮�� ����
        }
        // ���� �丮 ������ �����ͼ� �丮 ����

        var nextCookInfo = cookInfos.Dequeue();
        GameObject obj = GameObject.Instantiate(conFig.GameObjects[(int)CookEntityType.Food]);



        Cook cookComponent = obj.GetComponent<Cook>();
      
        for (int i = 0; i < conFig.Foods.Count; i++)
        {
            if (conFig.Foods[i].displayName == nextCookInfo.foodName)
            {
                cookComponent.WaitingTime = conFig.Foods[i].waitingTime;
                break;
            }
        }
        cookComponent.Setup(nextCookInfo, this);
        Cooks.Enqueue(cookComponent);
    }



    public override void Update()
    {
        TryStartNextCook(); // ���ο� �丮�� �ʿ��ϴٸ� ����
    }

    CookManagerConfig conFig;

    // �丮���� ��� �Ѱ��� ������ 
    public Queue<Cook> Cooks { get; set; } = new Queue<Cook>();

    // ���� �丮�� ���� �̸� ���
    Queue<CookInfo> cookInfos = new Queue<CookInfo>();

    List<GameObject> Cooker = new List<GameObject>();
}
