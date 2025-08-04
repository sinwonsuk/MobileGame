using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StaffShopButton : MonoBehaviour
{
    [Header("연결된 직원 데이터 (SO)")]
    public StaffStatsSO staffData;
    public RuntimeStaffStatsSO RuntimeStaffData;
    public Button purchaseButton;

    StaffStatsSO staff;
    StaffBase _spawnedStaff;

    public TextMeshProUGUI _buttonText;
    public TextMeshProUGUI level_num;

    public StaffType stafType;
    public string num1;
    public int money;

    void Awake()
    {
        money = staffData.baseSalary;
        // spawnPoint 관련 완전 삭제
    }

    void Start()
    {
        purchaseButton.onClick.AddListener(OnButtonClicked);
        RefreshUI();
    }

    // SpawnStaff 함수 전체 삭제

    private void RefreshUI()
    {
        level_num.text = $"Lv. {RuntimeStaffData.level}";

        int effectiveLevel = Mathf.Max(1, RuntimeStaffData.level);
        money = staffData.baseSalary * effectiveLevel;

        _buttonText.text = (RuntimeStaffData.level == 0) ? "Buy" : "Upgrade";
    }

    private void OnButtonClicked()
    {
        // 돈 차감
        EventBus<MoneyChangeMusHandler>.Raise(new MoneyChangeMusHandler(money));

        if (RuntimeStaffData.level == 0)
        {
            // 첫 구매
            RuntimeStaffData.level = 1;
            RuntimeStaffData.isOwned = true; // ← 최초 구매 시 true
        }
        else
        {
            RuntimeStaffData.level += 1; // 업그레이드 시 레벨 증가
            RuntimeStaffData.isOwned = true; // 이미 true겠지만 명확하게
            _spawnedStaff?.LevelUp();
        }
        EmployeeManager.Instance.NotifyStaffChanged();
        RefreshUI();
    }


}
