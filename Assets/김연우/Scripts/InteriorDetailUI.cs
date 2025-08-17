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

    private void ShowDetailFromSkin(InteriorSlot slot, int skinIndex, Sprite sprite)
    {
        currentSlot = slot;
        currentSkinIndex = skinIndex;

        panel.SetActive(true);
        nameText.text = slot.data.interiorName;
        descText.text = slot.data.description;
        iconImage.sprite = sprite != null ? sprite : slot.data.icon;

        useButton.GetComponentInChildren<TMP_Text>().text =
            slot.runtimeData.isUsed ? "해제" : "설치";
        useButton.onClick.RemoveAllListeners();
        useButton.onClick.AddListener(OnUseClicked);
        useButton.interactable = true;
    }

    private void OnUseClicked()
    {
        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click, false);
        if (currentSlot == null) return;

        // 설치/해제 토글 (내부 로직 그대로)
        InteriorManager.Instance.UseInterior(currentSlot.data.interiorName);

        // 버튼 라벨 갱신
        useButton.GetComponentInChildren<TMP_Text>().text =
            currentSlot.runtimeData.isUsed ? "해제" : "설치";
    }
}
