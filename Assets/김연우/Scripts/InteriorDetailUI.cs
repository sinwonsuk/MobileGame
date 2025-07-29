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
        Debug.Log("ShowDetail 호출됨 for: " + slot.data.interiorName);

    }

    private void OnUseClicked()
    {
        InteriorManager.Instance.UseInterior(currentSlot.data.interiorName);
    }
}
