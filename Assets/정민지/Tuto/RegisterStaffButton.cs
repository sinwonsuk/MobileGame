using UnityEngine;
using UnityEngine.UI;

public class RegisterStaffButton : MonoBehaviour
{
    void Start()
    {
        Button button = GetComponent<Button>();
        if (button != null)
        {
            TutorialManager.Instance?.RegisterButton("staffButton", button);
        }
    }
}
