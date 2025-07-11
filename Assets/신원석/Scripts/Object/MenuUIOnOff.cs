using UnityEngine;

public class MenuUIOnOff : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IsActive()
    {
        if (Check == false)
        {
            EventBus<MenuBoardActiveHandler>.Raise(new MenuBoardActiveHandler(true));
            Check = true;
        }         
        else
        {
            EventBus<MenuBoardActiveHandler>.Raise(new MenuBoardActiveHandler(false));
            Check = false;
        }
    }

    private bool Check = false;

}
