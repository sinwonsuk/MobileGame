using UnityEngine;
using UnityEngine.UI;

public class RegisterIn : MonoBehaviour
{
    void Start()
    {
        Button button = GetComponent<Button>();
        if (button != null)
        {
            TutorialManager.Instance?.RegisterButton("in", button);
        }
    }
}
