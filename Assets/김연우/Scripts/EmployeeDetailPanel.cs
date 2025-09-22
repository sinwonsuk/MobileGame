using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EmployeeDetailPanel : MonoBehaviour
{
    public Image previewImage;
    public TMP_Text nameText;
    public Button assignButton;
    public Button releaseButton;
    public Button[] numberButtons;
    public TMP_Text introduceText;

    private EmployeeSlot currentSlot;
    void Awake()
    {
        // 시작 시 디테일 패널 숨김
        if (gameObject.activeSelf) gameObject.SetActive(false);
    }
    public void Open(EmployeeSlot slot)
    {
        currentSlot = slot;
        previewImage.sprite = slot.staffData.portrait;
        nameText.text = slot.staffData.displayName;
        introduceText.text = slot.staffData.explain;

        assignButton.gameObject.SetActive(!slot.IsAssigned);
        releaseButton.gameObject.SetActive(slot.IsAssigned);

        assignButton.onClick.RemoveAllListeners();
        assignButton.onClick.AddListener(OnAssignClicked);

        releaseButton.onClick.RemoveAllListeners();
        releaseButton.onClick.AddListener(OnReleaseClicked);

        for (int i = 0; i < numberButtons.Length; i++)
        {
            int idx = i;
            numberButtons[i].onClick.RemoveAllListeners();
            numberButtons[i].onClick.AddListener(() => OnNumberButton(idx));
        }

        gameObject.SetActive(true);
    }

    void OnAssignClicked()
    {
        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click, false); // ← 추가
        TutorialManager.Instance?.TriggerEvent("in");
        EmployeeManager.Instance.StartPlacement(currentSlot);
        gameObject.SetActive(false);
    }

    void OnReleaseClicked()
    {
        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click, false); // ← 추가
        EmployeeManager.Instance.ReleaseEmployee(currentSlot);
        gameObject.SetActive(false);
    }

    void OnNumberButton(int idx)
    {
        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click, false); // ← 추가
        Debug.Log($"직원 {currentSlot.staffData.displayName} - {idx + 1}번 기능 (구현 예정)");
    }

}
