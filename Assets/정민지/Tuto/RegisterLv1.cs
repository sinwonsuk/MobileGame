using UnityEngine;
using UnityEngine.UI; // UGUI 버튼용

public class RegisterLv1 : MonoBehaviour
{
    void Start()
    {
        Button button = GetComponent<Button>();
        if (button != null)
        {
            TutorialManager.Instance?.RegisterButton("Lv1", button);
        }
        else
        {
            Debug.LogWarning("Button 컴포넌트가 없습니다!");
        }
    }
}
