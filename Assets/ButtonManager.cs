using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public enum ButtonClick
{
    none,
    Menu,
    Enhance,
    MenuBoard,
    Settings,
    Inven,
    Restaurant,
    hunter,
    Dungeon
}



public class ButtonManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (Buttons e in buttons)
        {
            if (e == null) 
                continue;

            if (!buttonDic.ContainsKey(e.type))
            {
                buttonDic.Add(e.type, e.button);
                e.button.ButtonClick = e.type;
            }
              
        }
    }

    void Update()
    {
        
    }


    public void OnClick(ButtonClick target)
    {
        if (!buttonDic.TryGetValue(target, out BaseButton next) || next == null)           
            return;


        // ���� ��ư�� �ٽ� ������ (�ݱ�)
        if (buttonClick == target)
        {
            next.OnExit();
            buttonClick = ButtonClick.none;
            return;
        }

        // �ٸ� ��ư�� ���������� �ݴ´ٸ� ������ ��ư exit
        if (buttonClick != ButtonClick.none && buttonDic.TryGetValue(buttonClick, out BaseButton prev) && prev != null)
            prev.OnExit();

        // �� ��ư ����
        next.OnClick();
        buttonClick = target;
    }


    ButtonClick buttonClick = ButtonClick.none;

    [System.Serializable]
    public class Buttons
    {
        public ButtonClick type;
        public BaseButton button; // IUIScreen ����ü
    }

    [SerializeField] private List<Buttons> buttons = new List<Buttons>();
    private Dictionary<ButtonClick, BaseButton> buttonDic = new();


}
