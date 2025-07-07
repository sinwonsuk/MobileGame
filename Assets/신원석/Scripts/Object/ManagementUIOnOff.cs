using UnityEngine;

public class ManagementUIOnOff : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ManagementUIActive()
    {

            EventBus<ManagementActiveHandler>.Raise(new ManagementActiveHandler(isActive));
            isActive = !isActive;         
    }

    bool isActive =true;


}
