using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public enum ButtonClick
{
    none,
    Menu,
    Enhance,
    MenuBoard,
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


        // 같은 버튼을 다시 누르면 (닫기)
        if (buttonClick == target)
        {
            next.OnExit();
            buttonClick = ButtonClick.none;
            return;
        }

        // 다른 버튼이 열려있을떄 닫는다면 이전꺼 버튼 exit
        if (buttonClick != ButtonClick.none && buttonDic.TryGetValue(buttonClick, out BaseButton prev) && prev != null)
            prev.OnExit();

        // 새 버튼 열기
        next.OnClick();
        buttonClick = target;
    }


    ButtonClick buttonClick = ButtonClick.none;

    [System.Serializable]
    public class Buttons
    {
        public ButtonClick type;
        public BaseButton button; // IUIScreen 구현체
    }

    [SerializeField] private List<Buttons> buttons = new List<Buttons>();
    private Dictionary<ButtonClick, BaseButton> buttonDic = new();


}
