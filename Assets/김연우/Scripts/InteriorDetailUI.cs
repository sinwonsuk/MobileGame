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

    private void OnEnable()
    {
        InteriorSlotUI.OnSlotClicked += ShowDetail;
    }

    private void OnDisable()
    {
        InteriorSlotUI.OnSlotClicked -= ShowDetail;
    }

    private void ShowDetail(InteriorSlot slot)
    {
        currentSlot = slot;
        panel.SetActive(true);
        nameText.text = slot.data.interiorName;
        descText.text = slot.data.description;
        iconImage.sprite = slot.data.icon;

        useButton.GetComponentInChildren<TMP_Text>().text = slot.runtimeData.isUsed ? "해제" : "설치";
        useButton.onClick.RemoveAllListeners();
        useButton.onClick.AddListener(OnUseClicked);

        // 필요 시 버튼 활성화
        useButton.interactable = true;

        Debug.Log("ShowDetail 호출됨 for: " + slot.data.interiorName);
    }

    private void OnUseClicked()
    {
        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click, false);

        if (currentSlot == null)
            return;

        // 지금 상태가 '해제' 버튼인지 검사 (해제 눌렀을 때만 초기화)
        bool wasUsed = currentSlot.runtimeData.isUsed;

        // 설치/해제 토글
        InteriorManager.Instance.UseInterior(currentSlot.data.interiorName);

        if (wasUsed)
        {
            // SellPanelUI처럼 해제 직후 1회 초기화
            ClearFields();
        }
        else
        {
            // 설치를 한 경우엔 버튼 라벨만 갱신
            useButton.GetComponentInChildren<TMP_Text>().text = "해제";
        }
    }

    // ▶ SellPanelUI의 ClearFields와 비슷한 초기화
    private void ClearFields()
    {
        currentSlot = null;

        nameText.text = string.Empty;
        descText.text = string.Empty;
        iconImage.sprite = null;

        // 버튼 비활성화 + 텍스트도 초기화
        useButton.interactable = false;
        useButton.GetComponentInChildren<TMP_Text>().text = string.Empty;
        useButton.onClick.RemoveAllListeners();
        // 패널은 그대로 둔다(닫고 싶으면 panel.SetActive(false)로 변경 가능)
    }
}
