// InteriorDetailUI.cs (핵심만 수정)
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InteriorDetailUI : MonoBehaviour
{
    public GameObject panel;
    public TMP_Text nameText;
    public TMP_Text descText;
    public Image iconImage;
    public Button useButton;

    private InteriorSlot currentSlot;
    private int currentSkinIndex = 0;

    private void OnEnable()
    {

        InteriorSkinSlotUI.OnSkinClicked += ShowDetailFromSkin;
    }

    private void OnDisable()
    {
        InteriorSkinSlotUI.OnSkinClicked -= ShowDetailFromSkin;
    }

    // InteriorDetailUI.cs
    private void ShowDetailFromSkin(InteriorSlot slot, int skinIndex, Sprite sprite)
    {
        currentSlot = slot;
        currentSkinIndex = skinIndex;

        panel.SetActive(true);
        nameText.text = slot.data.interiorName;
        descText.text = slot.data.description;
        iconImage.sprite = sprite != null ? sprite : slot.data.icon;

        // ★ 버튼 세팅
        useButton.onClick.RemoveAllListeners();

        bool isLocked = slot.data.alwaysInstalled; // 항상 설치(해제 불가) 여부
        var label = useButton.GetComponentInChildren<TMPro.TMP_Text>(true);

        if (isLocked)
        {
            if (label) label.text = "고정";
            useButton.interactable = false;

        }
        else
        {
            if (label) label.text = slot.runtimeData.isUsed ? "해제" : "설치";
            useButton.interactable = true;
            useButton.onClick.AddListener(OnUseClicked);
        }
    }


    private void OnUseClicked()
    {
        // ★ 항상 설치면 안전 차단
        if (currentSlot != null && currentSlot.data.alwaysInstalled)
            return;

        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click, false);
        if (currentSlot == null) return;

        InteriorManager.Instance.UseInterior(currentSlot.data.interiorName);

        // 라벨 갱신
        var label = useButton.GetComponentInChildren<TMPro.TMP_Text>(true);
        if (label) label.text = currentSlot.runtimeData.isUsed ? "해제" : "설치";
    }

}
