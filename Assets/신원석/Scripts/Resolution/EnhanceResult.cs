using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnhanceResult : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image image;


    public Image Image
    {
        get => image;
        set => image = value;
    }

    public TextMeshProUGUI Text
    {
        get => text;
        set => text = value;
    }

    private float time;

    private void OnDisable()
    {
        time = 0f;
        // 한 프레임 뒤에 호출되도록
        Invoke(nameof(RaiseToggleEvent), 0f);

        button.interactable = true;
    }

    private void RaiseToggleEvent()
    {
        EventBus<SetEnhanceFoodActiveEvent>.Raise(new SetEnhanceFoodActiveEvent());
    }

    private void Update()
    {
        time += Time.deltaTime;
        if (time > 1f)
        {
            gameObject.SetActive(false);
        }
        else
        {
            button.interactable = false;
        }

    }

    public void Click()
    {
        gameObject.SetActive(false);
    }

    [SerializeField] Button button;
}