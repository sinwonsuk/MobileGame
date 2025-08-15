using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class OrderCounter : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if(Input.GetMouseButton(0))
        //{
        //    EventBus<MoneyChangeMusHandler>.Raise(new MoneyChangeMusHandler(100));
        //    EventBus<MoneyChangePusHandler>.Raise(new MoneyChangePusHandler(100));
        //}
    }

    [SerializeField] List<Transform> queuePositions = new List<Transform>();

    public List<Transform> QueuePositions 
    { 
        get => queuePositions;
    }


}
