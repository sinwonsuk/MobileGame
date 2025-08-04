using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StaffShopButton : MonoBehaviour
{
    [Header("연결된 직원 데이터 (SO)")]
    public StaffStatsSO staffData;
    public RuntimeStaffStatsSO RuntimeStaffData;
    public Transform spawnPoint;
    public Button purchaseButton;

    StaffStatsSO staff;
    StaffBase _spawnedStaff;

    public TextMeshProUGUI _buttonText;
    public TextMeshProUGUI level_num;

    public StaffType stafType; // 타입 구분 (Restaurant, Hunter 등)
    public string num1;
    public int money;

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
            }
        }

        Debug.Log($"[{staffData.displayName}] num1 = {num1}, initial spawnPoint = {spawnPoint}", this);
        if (spawnPoint == null)
            Debug.LogWarning($"[{staffData.displayName}] SpawnStaff 불가 – spawnPoint가 null입니다.", this);

        if (RuntimeStaffData.level > 0 && stafType == StaffType.restaurant)
            SpawnStaff();
    }

    void Start()
    {
        purchaseButton.onClick.AddListener(OnButtonClicked);
        RefreshUI();
    }

    private void SpawnStaff()
    {
        // 오직 Restaurant 타입만 스폰
        if (stafType != StaffType.restaurant)
        {
            Debug.Log($"[{staffData.displayName}]은 Restaurant 타입이 아니어서 배치/스폰하지 않음.", this);
            return;
        }

        if (spawnPoint == null) return;
        var go = Instantiate(staffData.itemPrefab, spawnPoint.position, spawnPoint.rotation);
        _spawnedStaff = go.GetComponent<StaffBase>();
        _spawnedStaff.Init(staffData, RuntimeStaffData);
    }

    private void RefreshUI()
    {
        // 레벨 텍스트
        level_num.text = $"Lv. {RuntimeStaffData.level}";

        // 가격 계산
        int effectiveLevel = Mathf.Max(1, RuntimeStaffData.level);
        money = staffData.baseSalary * effectiveLevel;

        // 버튼 텍스트
        _buttonText.text = (RuntimeStaffData.level == 0) ? "Buy" : "Upgrade";
    }

    private void OnButtonClicked()
    {
        if (RuntimeStaffData.level == 0)
        {
            // 첫 구매
            RuntimeStaffData.level++;
            if (stafType == StaffType.restaurant)
            {
                SpawnStaff();
            }
            else
            {
                Debug.Log($"[{staffData.displayName}]은 Restaurant 타입이 아니어서 SpawnStaff를 실행하지 않음.", this);
            }
        }
        else
        {
            EventBus<MoneyChangeMusHandler>.Raise(new MoneyChangeMusHandler(money));
            if (_spawnedStaff != null)
                _spawnedStaff.LevelUp();
        }

        RefreshUI();
    }
}
