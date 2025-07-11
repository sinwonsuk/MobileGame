// == StaffShopButton.cs ==
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StaffShopButton : MonoBehaviour
{
    [Header("�Ҵ��� ���� ������(SO)")]
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
            // -- ù ���� --
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
            // -- ������(���׷��̵�) --
            _spawnedStaff.LevelUp();
            level_num.text = $"Lv. {staffData.level}";
        }
    }
}
