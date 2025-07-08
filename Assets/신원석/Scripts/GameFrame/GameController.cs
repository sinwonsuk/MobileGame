using System;
using System.Collections.Generic;
using UnityEngine;


public class GameController : MonoBehaviour
{
    private Dictionary<Type, baseManager> managerMap = new Dictionary<Type, baseManager>();
    private Dictionary<Type, BaseScriptableObject> dicBaseScriptableObject = new Dictionary<Type, BaseScriptableObject>();


    [SerializeField]
    private List<BaseScriptableObject> baseScriptableObjects = new List<BaseScriptableObject>();

    void Awake()
    {
        ConnectBaseScriptableObject();

        //Register<UIManager, UIManagerConfig>(Config => new UIManager(Config));
        Register<CustomerManager, CustomerManagerConfig>(Config => new CustomerManager(Config));
        Register<TableManager, TableManagerConfig>(Config => new TableManager(Config));
        Register<FoodSelectionManager, FoodSelectionManagerConfig>(Config => new FoodSelectionManager(Config));
        Register<FoodUIManager, FoodManagerConfig>(Config => new FoodUIManager(Config));


        InitAll();
        ActiveOffAll();
    }

    void ConnectBaseScriptableObject()
    {
        for (int i = 0; i < baseScriptableObjects.Count; i++)
        {
            dicBaseScriptableObject.Add(baseScriptableObjects[i].type, baseScriptableObjects[i]);
        }
    }

    private void Register<TManager, TConfig>(Func<TConfig, TManager> factory) where TManager : baseManager where TConfig : BaseScriptableObject
    {
    
        TConfig config = (TConfig)dicBaseScriptableObject[typeof(TConfig)];
        TManager manager = factory(config);
        // 제네릭은 new TManager 가 안되서 어쩔수없이 Func<TConfig, TManager> factory 사용 
        RegisterMap(manager);
    }

    private void RegisterMap<T1>(T1 manager) where T1 : baseManager
    {
        managerMap[typeof(T1)] = manager;
    }

    // 전부다 초기화 
    private void InitAll()
    {
        foreach (var manager in managerMap.Values)
        {
           manager.Init();
        }
    }

    private void ActiveOffAll()
    {
        foreach (var manager in managerMap.Values)
        {
            manager.ActiveOff();
        }
    }

    private void UpdateAll()
    {
        foreach (var manager in managerMap.Values)
        {
            manager.Update();
        }
    }


    // 혹시 몰라 만듬 
    public T GetManager<T>() where T : baseManager
    {
        return (T)managerMap[typeof(T)];

        // 사용 예시
        //GetManager<UIManager>().함수      
    }

    void Update()
    {

        UpdateAll();
    }
}