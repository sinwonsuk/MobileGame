using UnityEngine;
using UnityEngine.UI;

public class RegisterInteriorButton : MonoBehaviour
{
    void Start()
    {
        Button button = GetComponent<Button>();
        if (button != null)
        {
            TutorialManager.Instance.RegisterButton("interiorButton", button);
        }
    }
}
