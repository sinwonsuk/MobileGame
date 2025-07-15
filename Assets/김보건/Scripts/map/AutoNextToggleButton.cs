using UnityEngine;
using UnityEngine.UI;

public class AutoNextToggleButton : MonoBehaviour
{
    public Sprite onSprite;  // NextOn.png
    public Sprite offSprite; // NextOff.png
    public Image targetImage; // 버튼 이미지

    private bool isOn = false;

    void Start()
    {
        UpdateButtonVisual();
    }

    public void ToggleButton()
    {
        isOn = !isOn;
        UpdateButtonVisual();

        // 실제 설정 저장
        EventBus<AutoNextFloorChangedEvent>.Raise(new AutoNextFloorChangedEvent(isOn));

        Debug.Log($"[AutoNextToggleButton] autoNextFloor 값 변경됨: {isOn}");
    }

    private void UpdateButtonVisual()
    {
        if (isOn)
            targetImage.sprite = onSprite;
        else
            targetImage.sprite = offSprite;
    }

    public bool GetIsOn()
    {
        return isOn;
    }
}
