using UnityEngine;

public class MenuBoardExitClick : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick()
    {
        EventBus<MenuBoardActiveHandler>.Raise(new MenuBoardActiveHandler(false));
        ButtonManager.buttonClick = ButtonClick.none;
    }
}
