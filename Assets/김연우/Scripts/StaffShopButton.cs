using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class StaffShopButton : MonoBehaviour
{
    [Header("연결된 직원 데이터 (SO)")]
    public StaffStatsSO staffData;
    public RuntimeStaffStatsSO staffruntimeData;

    [Header("UI & 버튼")]
    public Button purchaseButton;
    public TextMeshProUGUI buttonText;
    public TextMeshProUGUI levelText;

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
/*        // 선택: MapPoint 자동 바인딩
        if (spawnPoint == null && staffType == StaffType.restaurant)
        {
            var parent = GameObject.Find("MapPoint");
            if (parent != null)
            {
                var t = parent.transform;
                if (num1 == "first" && t.childCount > 2) spawnPoint = t.GetChild(2);
                if (num1 == "second" && t.childCount > 3) spawnPoint = t.GetChild(3);
            }
        }*/
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
        int current = staffruntimeData != null ? staffruntimeData.level : 0;
        int nextLevel = (current == 0) ? 1 : current + 1;
        _price = (staffData != null) ? staffData.baseSalary * nextLevel : 0;

        if (levelText != null) levelText.text = $"Lv. {current}";
        if (buttonText != null) buttonText.text = (current == 0) ? "Buy" : "Upgrade";
    }

    private void OnClick()
    {
        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click, false);

        // 같은 프레임 중복 클릭 차단
        if (_lastUpgradeFrame == Time.frameCount) return;
        _lastUpgradeFrame = Time.frameCount;

        if (staffData == null || staffruntimeData == null)
        {
            Debug.LogError("[StaffShopButton] 데이터가 비어있습니다.", this);
            return;
        }

        // 같은 Runtime SO에 대한 동시 업그레이드 차단
        if (UpgradeLock.Contains(staffruntimeData)) return;
        UpgradeLock.Add(staffruntimeData);

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

            int prev = staffruntimeData.level;

            if (prev == 0) // ★ 첫 구매
            {
                staffruntimeData.level = 1;
                staffruntimeData.isOwned = true;
                staffruntimeData.isDirty = true;
                staffruntimeData.RecalcWith(staffData);
                // 레스토랑(경영) 직원은 구매=배치 동시 처리
                if (staffType == StaffType.restaurant)
                {
                    // 우선 EmployeeManager 통해 확정 인덱스로 배치 시도
                    int targetIndex = ResolveAssignIndex();

                    if (EmployeeManager.Instance != null && targetIndex >= 0)
                    {
                        // 매니저 유틸로 배치(내부에서 isAssigned/assignedIndex/isDirty 처리)
                        EmployeeManager.Instance.TryPlaceAtIndex(staffruntimeData, staffData, targetIndex);
                    }
                    else
                    {
                        // 매니저 없거나 인덱스 계산 실패 → 로컬 스폰 + 데이터표시
                        SafeSpawn();
                        // assigned/assignedIndex만 세팅 (DB 저장되도록 더티 유지)
                        staffruntimeData.isAssigned = true;
                        staffruntimeData.assignedIndex = targetIndex;
                        staffruntimeData.isDirty = true;     // ← 배치 더티
                    }
                }
            }
            else // ★ 업그레이드
            {
                staffruntimeData.level += 1;
                staffruntimeData.isOwned = true;
                staffruntimeData.isDirty = true;

                /*if (_spawned != null)
                    _spawned.LevelUp();*/
                staffruntimeData.RecalcWith(staffData);
            }

            // 외부 UI 갱신 통지(있으면)
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
        // 우선 고정 인덱스가 지정돼 있으면 그대로 사용
        if (autoAssignIndex >= 0) return autoAssignIndex;

        // spawnPoint가 MapPoint의 자식이면 siblingIndex로 계산
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
        // 실제 차감 (MoneyManager.UseMoney를 이벤트로 호출)
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

        // 같은 포인트에 기존 자식 정리(겹침 방지)
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
