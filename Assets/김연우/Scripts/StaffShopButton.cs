using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class StaffShopButton : MonoBehaviour
{
    [Header("연결된 직원 데이터 (SO)")]
    public StaffStatsSO staffData;
    public RuntimeStaffStatsSO runtimeData;

    [Header("UI & 버튼")]
    public Button purchaseButton;
    public TextMeshProUGUI buttonText;
    public TextMeshProUGUI levelText;

    [Header("스폰(레스토랑만 사용)")]
    public StaffType staffType;          // Restaurant / Hunter ...
    public Transform spawnPoint;         // MapPoint 하위(옵션)
    public string num1;                  // "first" / "second"

    // 내부 상태
    private StaffBase _spawned;
    private int _price;
    private bool _listenerBound;

    // --- 중복 업그레이드 가드 ---
    private static readonly HashSet<RuntimeStaffStatsSO> UpgradeLock = new();
    private int _lastUpgradeFrame = -1;

    private void Awake()
    {
        // 선택: MapPoint 자동 바인딩
        if (spawnPoint == null && staffType == StaffType.restaurant)
        {
            var parent = GameObject.Find("MapPoint");
            if (parent != null)
            {
                var t = parent.transform;
                if (num1 == "first" && t.childCount > 2) spawnPoint = t.GetChild(2);
                if (num1 == "second" && t.childCount > 3) spawnPoint = t.GetChild(3);
            }
        }
    }

    private void OnEnable()
    {
        if (purchaseButton != null && !_listenerBound)
        {
            purchaseButton.onClick.RemoveListener(OnClick); // 안전차단
            purchaseButton.onClick.AddListener(OnClick);
            _listenerBound = true;
            Debug.Log($"[Bind] {name} -> BtnID {purchaseButton.GetInstanceID()}");
        }
        RefreshUI();
    }

    private void OnDisable()
    {
        if (purchaseButton != null)
            purchaseButton.onClick.RemoveListener(OnClick);
        _listenerBound = false;
    }

    private void RefreshUI()
    {
        int current = runtimeData != null ? runtimeData.level : 0;
        int nextLevel = (current == 0) ? 1 : current + 1;
        _price = (staffData != null) ? staffData.baseSalary * nextLevel : 0;

        if (levelText != null) levelText.text = $"Lv. {current}";
        if (buttonText != null) buttonText.text = (current == 0) ? "Buy" : "Upgrade";
    }

    private void OnClick()
    {
        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click, false); // ← 추가
        // 같은 프레임 중복 클릭 차단
        if (_lastUpgradeFrame == Time.frameCount) return;
        _lastUpgradeFrame = Time.frameCount;

        if (staffData == null || runtimeData == null)
        {
            Debug.LogError("[StaffShopButton] 데이터가 비어있습니다.", this);
            return;
        }

        // 같은 Runtime SO에 대한 동시 업그레이드 차단
        if (UpgradeLock.Contains(runtimeData)) return;
        UpgradeLock.Add(runtimeData);

        try
        {
            // 돈 선확인
            if (!CanAfford(_price))
            {
                Debug.Log("돈 부족");
                return;
            }

            // 실제 차감
            Spend(_price);

            // 레벨 갱신(항상 +1)
            int prev = runtimeData.level;
            if (prev == 0)
            {
                runtimeData.level = 1;
                runtimeData.isOwned = true;

                if (staffType == StaffType.restaurant)
                    SafeSpawn();
            }
            else
            {
                runtimeData.isOwned = true;

                if (_spawned != null && staffType == StaffType.restaurant)
                    _spawned.LevelUp();
            }

            // 외부 UI 갱신 통지(있으면)
            if (EmployeeManager.Instance != null)
                EmployeeManager.Instance.NotifyStaffChanged();

            RefreshUI();
        }
        finally
        {
            UpgradeLock.Remove(runtimeData);
        }
    }

    private bool CanAfford(int amount)
    {
        // 프로젝트 기준으로 소지금 확인
        int current = BackendGameData.Instance.userData.gold;
        return current >= amount;
    }

    private void Spend(int amount)
    {
        // 실제 차감 (MoneyManager.UseMoney를 이벤트로 호출)
        EventBus<MoneyChangeMusHandler>.Raise(new MoneyChangeMusHandler(amount));
    }

    private void SafeSpawn()
    {
        if (spawnPoint == null)
        {
            Debug.LogWarning("[StaffShopButton] spawnPoint가 설정되지 않았습니다.", this);
            return;
        }
        if (staffData == null || staffData.itemPrefab == null)
        {
            Debug.LogError("[StaffShopButton] itemPrefab이 비어있습니다.", this);
            return;
        }

        var go = Instantiate(staffData.itemPrefab, spawnPoint.position, spawnPoint.rotation);

        _spawned = go.GetComponent<StaffBase>();
        if (_spawned == null)
        {
            Debug.LogError("[StaffShopButton] itemPrefab에 StaffBase가 없습니다.", go);
            return;
        }

        _spawned.Init(staffData, runtimeData);
    }
}
