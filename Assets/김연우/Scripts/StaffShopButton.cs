// == StaffShopButton.cs ==
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StaffShopButton : MonoBehaviour
{
    [Header("할당할 직원 데이터(SO)")]
    public StaffStatsSO staffData;
    public Transform spawnPoint;
    public Button purchaseButton;

    StaffBehavior _spawnedStaff;
    public TextMeshProUGUI _buttonText;
    public TextMeshProUGUI level_num;
    void Start()
    {
        level_num.text = $"Lv. 0";
        purchaseButton.onClick.AddListener(OnButtonClicked);
    }

    void OnButtonClicked()
    {
        if (_spawnedStaff == null)
        {
            // -- 첫 구매 --
            var go = Instantiate(staffData.itemPrefab,
                                 spawnPoint.position,
                                 spawnPoint.rotation);
            _spawnedStaff = go.GetComponent<StaffBehavior>();
            _spawnedStaff.Init(staffData);

            _buttonText.text = "Upgrade";
            level_num.text = $"Lv. {staffData.level}";
        }
        else
        {
            // -- 레벨업(업그레이드) --
            _spawnedStaff.LevelUp();
            level_num.text = $"Lv. {staffData.level}";
        }
    }
}
