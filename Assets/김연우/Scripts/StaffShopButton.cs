using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StaffShopButton : MonoBehaviour
{
    [Header("연결된 직원 데이터 (SO)")]
    public StaffStatsSO staffData;
    public RuntimeStaffStatsSO RuntimeStaffData;
    public Transform spawnPoint;           // 스폰 위치(레스토랑 직원만 사용)
    public Button purchaseButton;

    public TextMeshProUGUI _buttonText;
    public TextMeshProUGUI level_num;

    public StaffType stafType;  // 이 직원의 타입
    public int money;
    public string num1;
    private StaffBase _spawnedStaff;

    void Awake()
    {
        money = staffData.baseSalary;
        // 스폰 포인트가 지정되지 않았다면 MapPoint에서 찾아서 할당
        if (spawnPoint == null)
        {
            var parent = GameObject.Find("MapPoint");

            switch (num1)
            {
                case "first":
                    if (parent != null && parent.transform.childCount > 2)
                        spawnPoint = parent.transform.GetChild(2);
                    break;
                case "second":
                    if (parent != null && parent.transform.childCount > 3)
                        spawnPoint = parent.transform.GetChild(3);
                    break;
                default:
                    break;
            }
        }
    }

    void Start()
    {
        purchaseButton.onClick.AddListener(OnButtonClicked);
        RefreshUI();
    }

    private void RefreshUI()
    {
        level_num.text = $"Lv. {RuntimeStaffData.level}";
        int effectiveLevel = Mathf.Max(1, RuntimeStaffData.level);
        money = staffData.baseSalary * effectiveLevel;
        _buttonText.text = (RuntimeStaffData.level == 0) ? "Buy" : "Upgrade";
    }

    private void OnButtonClicked()
    {
        // 돈 차감 이벤트 (공통)
        EventBus<MoneyChangeMusHandler>.Raise(new MoneyChangeMusHandler(money));

        if (RuntimeStaffData.level == 0)
        {
            // 첫 구매
            RuntimeStaffData.level = 1;
            RuntimeStaffData.isOwned = true;

            // 레스토랑 직원만 스폰!
            if (stafType == StaffType.restaurant)
            {
                SpawnStaff();
            }
        }
        else
        {
            RuntimeStaffData.level += 1;
            RuntimeStaffData.isOwned = true;

            // 업그레이드 시 스폰된 직원이 있으면 레벨업 처리 (레스토랑 직원만)
            if (_spawnedStaff != null && stafType == StaffType.restaurant)
                _spawnedStaff.LevelUp();
        }

        EmployeeManager.Instance.NotifyStaffChanged(); // 인벤토리 갱신
        RefreshUI();
    }

    // 오직 레스토랑 직원만 이 함수 사용!
    private void SpawnStaff()
    {
        if (spawnPoint == null)
        {
            Debug.LogWarning($"{staffData.displayName}: spawnPoint가 지정되지 않음", this);
            return;
        }
        var go = Instantiate(staffData.itemPrefab, spawnPoint.position, spawnPoint.rotation);
        _spawnedStaff = go.GetComponent<StaffBase>();
        _spawnedStaff.Init(staffData, RuntimeStaffData);
    }
}
