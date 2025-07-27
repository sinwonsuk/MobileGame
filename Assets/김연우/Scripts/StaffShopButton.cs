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

    public StaffType staff123;
    public string num1;
    public int money;

    void Awake()
    {
        money = staffData.baseSalary;
        // --- 디버깅 로그 추가 ---
        Debug.Log($"[{staffData.displayName}] num1 = {num1}, initial spawnPoint = {spawnPoint}", this);
        // 스폰 포인트가 지정되지 않았다면
        if (spawnPoint == null)
        {
            var parent = GameObject.Find("MapPoint");

            switch (num1)
            {
                case "first":
                    if (parent != null && parent.transform.childCount > 0)
                        spawnPoint = parent.transform.GetChild(0);
                    else
                        Debug.LogWarning("MapPoint가 없거나 자식이 없습니다.");
                    break;

                case "second":
                    if (parent != null && parent.transform.childCount > 1)
                        spawnPoint = parent.transform.GetChild(1);
                    else
                        Debug.LogWarning("MapPoint가 없거나 자식이 없습니다.");
                    break;

                case "third":
                    if (parent != null && parent.transform.childCount > 2)
                        spawnPoint = parent.transform.GetChild(2);
                    else
                        Debug.LogWarning("MapPoint가 없거나 자식이 없습니다.");
                    break;

                case "forth":
                    if (parent != null && parent.transform.childCount > 3)
                        spawnPoint = parent.transform.GetChild(3);
                    else
                        Debug.LogWarning("MapPoint가 없거나 자식이 없습니다.");
                    break;
                case "fifth":
                    if (parent != null && parent.transform.childCount > 4)
                        spawnPoint = parent.transform.GetChild(4);
                    else
                        Debug.LogWarning("MapPoint가 없거나 자식이 없습니다.");
                    break;
                case "sixth":
                    if (parent != null && parent.transform.childCount > 5)
                        spawnPoint = parent.transform.GetChild(5);
                    else
                        Debug.LogWarning("MapPoint가 없거나 자식이 없습니다.");
                    break;
            }
        }
        // 여전히 null 이면 경고
        if (spawnPoint == null)
            Debug.LogWarning($"[{staffData.displayName}] SpawnStaff 불가 – spawnPoint가 null입니다.", this);
        if (RuntimeStaffData.level > 0)
            SpawnStaff();
    }

    void Start()
    {
        RuntimeStaffData.level = 0;
        // 2) 버튼 리스너 등록 & UI 초기화
        purchaseButton.onClick.AddListener(OnButtonClicked);
        RefreshUI();
    }
    private void SpawnStaff()
    {
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
        // 돈 차감
        EventBus<MoneyChangeMusHandler>.Raise(new MoneyChangeMusHandler(money));

        if (RuntimeStaffData.level == 0)
        {
            // 첫 구매
            RuntimeStaffData.level = 1;
            SpawnStaff();
        }
        else
        {
            _spawnedStaff.LevelUp();
        }

        RefreshUI();
    }
}

