using UnityEngine;

public class MenuUIOnOff : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {       
        EventBus<ButtonisActiveHandler>.OnEvent += ManagementButtonisActive;
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

    public void ManagementButtonisActive(ButtonisActiveHandler buttonHandler)
    {
        gameObject.SetActive(buttonHandler.isActive);
    }


    private bool Check = false;

}
