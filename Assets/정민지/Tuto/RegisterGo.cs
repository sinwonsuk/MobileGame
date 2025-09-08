using UnityEngine;
using UnityEngine.UI; // UGUI ¹öÆ°¿ë

public class RegisterGo : MonoBehaviour
{
    void Start()
    {
        Button button = GetComponent<Button>();
        if (button != null)
        {
            TutorialManager.Instance?.RegisterButton("go", button);
        }
    }
}
