using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class StaffShopButton : MonoBehaviour
{
    private const int MAX_LEVEL = 50;

    // ★ 가격 증가율 상수(요청: 1.15배)
    private const float PRICE_GROWTH = 1.15f;

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

    [Header("전투 스탯 표시(UI)")]
    public TextMeshProUGUI atkText;   // 공격력
    public TextMeshProUGUI aspdText;  // 공격속도(초당 발사)

    [Header("경영 스탯 표시(UI)")]
    public TextMeshProUGUI workTimeText;   // 타임(실행시간)
    public TextMeshProUGUI restTimeText;   // 타이머(쉬는시간)

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

    // ★ 가격 계산 로직을 함수로 분리(1.15배 누적)
    private int CalcPriceForNextLevel()
    {
        if (staffData == null || staffruntimeData == null) return 0;

        int current = staffruntimeData.level;
        if (current >= MAX_LEVEL) return 0;

        // 다음에 살 레벨(0->1, n->n+1)
        int nextLevel = (current == 0) ? 1 : current + 1;

        // baseSalary × (1.15^(nextLevel-1))
        double multiplier = System.Math.Pow(PRICE_GROWTH, nextLevel - 1);
        int price = Mathf.RoundToInt(staffData.baseSalary * (float)multiplier);
        return price < 0 ? 0 : price; // 안전 보정
    }

    private void RefreshUI()
    {
        int current = staffruntimeData != null ? staffruntimeData.level : 0;

        // ★ 여기서도 동일한 함수 사용
        _price = CalcPriceForNextLevel();

        if (levelText != null)
            levelText.text = IsMaxLevel() ? $"Lv. {MAX_LEVEL} (MAX)" : $"Lv. {current}";

        if (nameText != null && staffData != null)
            nameText.text = staffData.displayName;

        if (buttonText != null)
        {
            buttonText.text = IsMaxLevel() ? "최대" : (current == 0 ? "구매" : "레벨 업");
            if (priceText != null) priceText.text = $"가격 : {_price}G";
        }

        if (purchaseButton != null) purchaseButton.interactable = !IsMaxLevel();

        // 타입별 스탯 UI 토글/갱신
        if (staffData != null && staffData.staffType == StaffType.hunter)
        {
            // 전투: 보이기
            if (atkText) atkText.gameObject.SetActive(true);
            if (aspdText) aspdText.gameObject.SetActive(true);
            // 경영: 숨기기
            if (workTimeText) workTimeText.gameObject.SetActive(false);
            if (restTimeText) restTimeText.gameObject.SetActive(false);

            double atk = staffruntimeData ? staffruntimeData.attack_Power : 0;
            double aspd = staffruntimeData ? staffruntimeData.attack_Speed : 0;

            if (atkText) atkText.text = $"공격력 : {atk:0}";
            if (aspdText) aspdText.text = $"공격속도 : {aspd:0.##}/s";
        }
        else if (staffData != null && staffData.staffType == StaffType.restaurant)
        {
            // 경영: 보이기
            if (workTimeText) workTimeText.gameObject.SetActive(true);
            if (restTimeText) restTimeText.gameObject.SetActive(true);
            // 전투: 숨기기
            if (atkText) atkText.gameObject.SetActive(false);
            if (aspdText) aspdText.gameObject.SetActive(false);

            double work = staffruntimeData ? staffruntimeData.timer : 0;   // 실행시간
            double rest = staffruntimeData ? staffruntimeData.cooltime : 0; // 쉬는시간

            if (workTimeText) workTimeText.text = $"노동시간 : {work:0.#}s";
            if (restTimeText) restTimeText.text = $"쉬는시간 : {rest:0.#}s";
        }
        else
        {
            // 아무 타입도 아니면 모두 숨김
            if (atkText) atkText.gameObject.SetActive(false);
            if (aspdText) aspdText.gameObject.SetActive(false);
            if (workTimeText) workTimeText.gameObject.SetActive(false);
            if (restTimeText) restTimeText.gameObject.SetActive(false);
        }
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
            // ★ 클릭 시점에서도 최신 가격 재계산(안전)
            _price = CalcPriceForNextLevel();

            if (!CanAfford(_price))
            {
                PopupManager.Show("돈이 부족합니다");
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
                EmployeeManager.Instance?.NotifyStaffChanged();
                staffruntimeData.RecalcWith(staffData);
                RefreshUI();

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
        BackendGameData.Instance.AddReputation(1);
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
