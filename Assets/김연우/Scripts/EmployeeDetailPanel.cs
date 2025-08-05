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

    private EmployeeSlot currentSlot;

    public void Open(EmployeeSlot slot)
    {
        currentSlot = slot;
        previewImage.sprite = slot.staffData.portrait;
        nameText.text = slot.staffData.displayName;

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
        EmployeeManager.Instance.StartPlacement(currentSlot);
        gameObject.SetActive(false); // 디테일 패널 닫기
    }

    void OnReleaseClicked()
    {
        EmployeeManager.Instance.ReleaseEmployee(currentSlot);
        gameObject.SetActive(false); // 디테일 패널 닫기
    }


    void OnNumberButton(int idx)
    {
        Debug.Log($"직원 {currentSlot.staffData.displayName} - {idx + 1}번 기능 (구현 예정)");
    }
}
