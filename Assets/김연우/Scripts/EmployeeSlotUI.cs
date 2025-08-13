using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;

public class EmployeeSlotUI : MonoBehaviour, IPointerClickHandler
{
    public Image iconImage;
    public TMP_Text nameText;
    public TMP_Text statusText;

    private EmployeeSlot slot;
    public static event Action<EmployeeSlot> OnSlotClicked;

    public void SetSlot(EmployeeSlot slot)
    {
        this.slot = slot;
        if (iconImage != null)
            iconImage.sprite = slot.staffData.portrait; // 직원 미리보기 이미지
        nameText.text = slot.staffData.displayName;
        statusText.text = slot.IsAssigned ? "배치중" : "";
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (slot != null)
        {
            SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click, false); 
            OnSlotClicked?.Invoke(slot);
        }
    }

}
