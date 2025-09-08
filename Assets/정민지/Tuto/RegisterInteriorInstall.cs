using UnityEngine;
using UnityEngine.UI;

public class RegisterInteriorInstall : MonoBehaviour
{
    void Start()
    {
        Button button = GetComponent<Button>();
        if (button != null)
        {
            TutorialManager.Instance?.RegisterButton("interiorInstallButton", button);
        }
    }
}
