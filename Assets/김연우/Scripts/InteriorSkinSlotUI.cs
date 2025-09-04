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

    public static event Action<InteriorSlot, int, Sprite> OnSkinClicked;


    public void Setup(InteriorSlot s, int index, Sprite sprite)
    {
        slot = s;
        skinIndex = index;
        if (image == null) image = GetComponent<Image>();
        if (image != null) image.sprite = sprite;

        // ★ index >= 1이면 클릭 불가 처리
        if (skinIndex >= 1)
        {
            // 클릭 막기
            var btn = GetComponent<Button>();
            if (btn != null) btn.interactable = false;

            // 색 어둡게 (시각적 구분용)
            if (image != null) image.color = new Color(1f, 1f, 1f, 0.4f);
        }
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (skinIndex >= 1) return; // ★ 클릭 무시

        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click, false);
        var spr = (image != null) ? image.sprite : null;
        OnSkinClicked?.Invoke(slot, skinIndex, spr);
        TutorialManager.Instance.TriggerEvent("TouchInterior");
    }

}
