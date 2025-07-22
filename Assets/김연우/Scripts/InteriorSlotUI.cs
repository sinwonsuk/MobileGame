using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class InteriorSlotUI : MonoBehaviour, IPointerClickHandler
{
    public Image iconImage;
    private InteriorSlot slot;
    public static event Action<InteriorSlot> OnSlotClicked;

    public void SetSlot(InteriorSlot s)
    {
        slot = s;
        iconImage.sprite = s.data.icon;
        // (원한다면 설치중 표시 UI 추가)
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnSlotClicked?.Invoke(slot);
        Debug.Log("Slot clicked: " + slot.data.interiorName);

    }
}
