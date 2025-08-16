using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class StaffShopButton : MonoBehaviour
{
    private const int MAX_LEVEL = 50;

    [Header("연결된 직원 데이터 (SO)")]
    public StaffStatsSO staffData;
    public RuntimeStaffStatsSO staffruntimeData;

    [Header("UI & 버튼")]
    public Button purchaseButton;
    public TextMeshProUGUI buttonText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI priceText;
    [Header("스폰(레스토랑만 사용)")]
    public StaffType staffType;
    public Transform spawnPoint;
    public string num1;

    [Header("자동 배치(선택)")]
    public int autoAssignIndex = -1;

    // 내부 상태
    private StaffBase _spawned;
    private int _price;
    private bool _listenerBound;

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

    private bool IsMaxLevel()
    {
        int lv = staffruntimeData != null ? staffruntimeData.level : 0;
        return lv >= MAX_LEVEL;
    }

    private void RefreshUI()
    {
        int current = staffruntimeData != null ? staffruntimeData.level : 0;

        int nextLevel = (current == 0) ? 1 : current + 1;
        int nextLevelClamped = Mathf.Clamp(nextLevel, 1, MAX_LEVEL);

        _price = (staffData != null && current < MAX_LEVEL)
            ? staffData.baseSalary * nextLevelClamped
            : 0;

        // 레벨 표기
        if (levelText != null)
            levelText.text = IsMaxLevel() ? $"Lv. {MAX_LEVEL} (MAX)" : $"Lv. {current}";

        // 이름 표시
        if (nameText != null && staffData != null)
            nameText.text = staffData.displayName;

        // 버튼 텍스트에 가격만 표시
        if (buttonText != null)
        {
            if (IsMaxLevel())
                buttonText.text = "최대";
            else
                buttonText.text = $"{_price}";
        }

        if (purchaseButton != null)
            purchaseButton.interactable = !IsMaxLevel();
    }


    private void OnClick()
    {
        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click, false);

        if (_lastUpgradeFrame == Time.frameCount) return;
        _lastUpgradeFrame = Time.frameCount;

        if (staffData == null || staffruntimeData == null)
        {
            Debug.LogError("[StaffShopButton] 데이터가 비어있습니다.", this);
            return;
        }

        // 이미 만렙이면 아무 것도 하지 않음
        if (IsMaxLevel())
        {
            Debug.Log("[StaffShopButton] 이미 최대 레벨입니다.");
            return;
        }

        if (UpgradeLock.Contains(staffruntimeData)) return;
        UpgradeLock.Add(staffruntimeData);

        try
        {
            if (!CanAfford(_price))
            {
                Debug.Log("돈 부족");
                return;
            }

            Spend(_price);

            int prev = staffruntimeData.level;

            if (prev == 0) // 첫 구매 -> 1레벨
            {
                staffruntimeData.level = 1;
                staffruntimeData.isOwned = true;
                staffruntimeData.isDirty = true;
                staffruntimeData.RecalcWith(staffData);

                if (staffType == StaffType.restaurant)
                {
                    int targetIndex = ResolveAssignIndex();

                    if (EmployeeManager.Instance != null && targetIndex >= 0)
                    {
                        EmployeeManager.Instance.TryPlaceAtIndex(staffruntimeData, staffData, targetIndex);
                    }
                    else
                    {
                        SafeSpawn();
                        staffruntimeData.isAssigned = true;
                        staffruntimeData.assignedIndex = targetIndex;
                        staffruntimeData.isDirty = true;
                    }
                }
            }
            else // 업그레이드 -> +1, 단 MAX_LEVEL 넘지 않게
            {
                staffruntimeData.level = Mathf.Min(prev + 1, MAX_LEVEL);
                staffruntimeData.isOwned = true;
                staffruntimeData.isDirty = true;
                staffruntimeData.RecalcWith(staffData);
            }

            EmployeeManager.Instance?.NotifyStaffChanged();
            RefreshUI();
        }
        finally
        {
            UpgradeLock.Remove(staffruntimeData);
        }
    }

    private int ResolveAssignIndex()
    {
        if (autoAssignIndex >= 0) return autoAssignIndex;
        if (spawnPoint != null && spawnPoint.parent != null)
            return spawnPoint.GetSiblingIndex();
        return -1;
    }

    private bool CanAfford(int amount)
    {
        int current = BackendGameData.Instance.userData.gold;
        return current >= amount;
    }

    private void Spend(int amount)
    {
        EventBus<MoneyChangeMusHandler>.Raise(new MoneyChangeMusHandler(amount));
        BackendGameData.Instance.userData.reputation += 1;
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

        for (int i = spawnPoint.childCount - 1; i >= 0; i--)
            Destroy(spawnPoint.GetChild(i).gameObject);

        var go = Instantiate(staffData.itemPrefab, spawnPoint.position, spawnPoint.rotation, spawnPoint);
        _spawned = go.GetComponent<StaffBase>();
        if (_spawned == null)
        {
            Debug.LogError("[StaffShopButton] itemPrefab에 StaffBase가 없습니다.", go);
            return;
        }
        _spawned.Init(staffData, staffruntimeData);
    }
}
