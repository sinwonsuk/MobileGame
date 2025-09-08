using UnityEngine;
using UnityEngine.UI;

public class RegisterInvenButton : MonoBehaviour
{
    void Start()
    {
        Button button = GetComponent<Button>();
        if (button != null)
        {
            TutorialManager.Instance?.RegisterButton("InvenButton", button);
        }
    }
}
