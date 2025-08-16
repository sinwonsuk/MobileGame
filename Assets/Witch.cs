using System;
using System.Collections.Generic;
using UnityEngine;

public enum Skin
{
    Idle,
    Sea,
}

public class Witch : MonoBehaviour
{

    private void OnEnable()
    {
        EventBus<ChangeSkinHandler>.OnEvent += ChangeSkin;
    }

    private void OnDisable()
    {
        EventBus<ChangeSkinHandler>.OnEvent -= ChangeSkin;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created


    // Update is called once per frame
    void Update()
    {
        
    }

    [SerializeField] private List<WitchSkin> skins = new List<WitchSkin>();


    [Serializable]
    public class WitchSkin
    {
        public Skin skin;
        public RuntimeAnimatorController animator;
    }

    public void ChangeSkin(ChangeSkinHandler skin)
    {
        foreach (var witchSkin in skins)
        {
            if (witchSkin.skin == skin.skin)
            {
                GetComponent<Animator>().runtimeAnimatorController = witchSkin.animator;
                return;
            }
        }
    }
}
