using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using TMPro;

public class InteriorSlotUI : MonoBehaviour, IPointerClickHandler
{
    public Image iconImage;
    public TMP_Text descText;
    private InteriorSlot slot;
    public static event Action<InteriorSlot> OnSlotClicked;

    public void SetSlot(InteriorSlot s)
    {
        slot = s;
        iconImage.sprite = s.data.icon;
        descText.text = s.data.description;
        // (원한다면 설치중 표시 UI 추가)
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click, false);
        OnSlotClicked?.Invoke(slot);
        Debug.Log("Slot clicked: " + slot.data.interiorName);

    }
}
