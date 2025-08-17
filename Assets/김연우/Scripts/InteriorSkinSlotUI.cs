// SkinSlotUI.cs
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class InteriorSkinSlotUI : MonoBehaviour, IPointerClickHandler
{
    public Image image; // 비워두면 자동 획득
    private InteriorSlot slot;
    private int skinIndex;      // 0부터 (0=기본)

    // 디테일창에게 알림: 어떤 인테리어의 몇 번 스킨을 눌렀는가 + 미리보기 스프라이트
    public static event Action<InteriorSlot, int, Sprite> OnSkinClicked;

    public void Setup(InteriorSlot s, int index, Sprite sprite)
    {
        slot = s;
        skinIndex = index;
        if (image == null) image = GetComponent<Image>();
        if (image != null) image.sprite = sprite;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click, false);
        var spr = (image != null) ? image.sprite : null;

        // 디테일창 띄우기(스킨 기준)
        OnSkinClicked?.Invoke(slot, skinIndex, spr);
    }
}
