using TMPro;
using UnityEngine;
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

    private void Awake()
    {
        // ★ 비활성화되어도 구독 유지
        InteriorSkinSlotUI.OnSkinClicked += ShowDetailFromSkin;
    }

    private void OnDestroy()
    {
        InteriorSkinSlotUI.OnSkinClicked -= ShowDetailFromSkin;
    }

    private void Start()
    {
        gameObject.SetActive(false);   // ★ 처음엔 통째로 꺼두기(원하던 동작)
        // panel은 굳이 끌 필요 없음. 어차피 부모가 꺼져 있으니까
    }

    private void ShowDetailFromSkin(InteriorSlot slot, int skinIndex, Sprite sprite)
    {
        gameObject.SetActive(true);    // ★ 부모 다시 켜기
        if (panel != null) panel.SetActive(true);

        currentSlot = slot;
        currentSkinIndex = skinIndex;

        nameText.text = slot.data.interiorName;
        descText.text = slot.data.description;
        iconImage.sprite = sprite != null ? sprite : slot.data.icon;

        useButton.onClick.RemoveAllListeners();

        bool isLocked = slot.data.alwaysInstalled;
        var label = useButton.GetComponentInChildren<TMPro.TMP_Text>(true);

        if (isLocked)
        {
            if (label) label.text = "고정";
            useButton.interactable = false;
        }
        else
        {
            if (slot.runtimeData.isUsed)
            {
                if (label) label.text = "설치됨";
                useButton.interactable = false;
            }
            else
            {
                if (label) label.text = "설치";
                useButton.interactable = true;
                useButton.onClick.AddListener(OnUseClicked);
            }
        }
    }

    private void OnUseClicked()
    {
        if (currentSlot == null) return;
        if (currentSlot.data.alwaysInstalled) return;

        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click, false);
        InteriorManager.Instance.UseInterior(currentSlot.data.interiorName);

        var label = useButton.GetComponentInChildren<TMPro.TMP_Text>(true);
        if (label) label.text = "설치됨";
        useButton.interactable = false;
    }
}
