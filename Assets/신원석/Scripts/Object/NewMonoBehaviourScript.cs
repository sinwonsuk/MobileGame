using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class aaaa : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        adRef.Value =10;

        keyValuePairs.Add("¸Ó¸´°í±â", 10);
    }

    // Update is called once per frame
    void Update()
    {
        if(adadad ==false)
        {
            EventBus<FoodQuantityChangedEvent>.Raise(new FoodQuantityChangedEvent(this));
            adadad = true;
        }

        Dictionary<string, int> kedsyValuePairs = keyValuePairs;

        int test = ad;


        FoodData adadafff = adada;

    }

    public Dictionary<string, int> keyValuePairs = new Dictionary<string, int>();

    [SerializeField] FoodData adada;

    public int ad = 10;

    bool adadad = false;

    public MutableInt adRef = new MutableInt();
}
