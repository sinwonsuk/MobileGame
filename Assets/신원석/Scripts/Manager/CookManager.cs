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
                cookComponent.WaitingTime = conFig.Foods[i].cookingTime;
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
