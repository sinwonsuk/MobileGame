using UnityEngine;

public abstract class BaseButton : MonoBehaviour
{
    public abstract void OnClick();  
    public abstract void OnExit();   
    public ButtonClick ButtonClick { get; set; }

}
