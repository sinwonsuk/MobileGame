using UnityEngine;
using UnityEngine.UI;

public class AutoNextToggleButton : MonoBehaviour
{
    public Sprite onSprite;  // NextOn.png
    public Sprite offSprite; // NextOff.png
    public Image targetImage; // ��ư �̹���

    private bool isOn = false;

    void Start()
    {
        UpdateButtonVisual();
    }

    public void ToggleButton()
    {
        isOn = !isOn;
        UpdateButtonVisual();

        // ���� ���� ����
        EventBus<AutoNextFloorChangedEvent>.Raise(new AutoNextFloorChangedEvent(isOn));

        Debug.Log($"[AutoNextToggleButton] autoNextFloor �� �����: {isOn}");
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
