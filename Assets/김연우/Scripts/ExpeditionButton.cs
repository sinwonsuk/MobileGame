using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ExpeditionButton : MonoBehaviour
{
    [Header("연결할 런타임 파견 데이터")]
    public RuntimeExpeditionSO runtimeSO;

    [Header("UI")]
    public Button mainBtn;
    public TMP_Text btnText;
    public TMP_Text timerText;
    public TMP_Text stateText;
    public TMP_Text nameText;

    public int requiredReputation = 0;

    private void Awake()
    {
        if (mainBtn != null) mainBtn.onClick.AddListener(OnMainBtnClicked);
    }

    private void OnEnable()
    {
        if (ExpeditionManager.Instance != null)
            ExpeditionManager.Instance.OnChanged += OnExpeditionChanged;

        RefreshUI();
        RefreshName();
    }

    private void OnDisable()
    {
        if (ExpeditionManager.Instance != null)
            ExpeditionManager.Instance.OnChanged -= OnExpeditionChanged;
    }

    private void Update()
    {
        RefreshTimer();
    }

    private void OnMainBtnClicked()
    {
        if (ExpeditionManager.Instance == null || runtimeSO == null) return;
        var id = runtimeSO.indate;
        if (string.IsNullOrEmpty(id)) return;

        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click, false);

        // 1) 대기 중 → 시작
        if (ExpeditionManager.Instance.CanStart(id))
        {
            ExpeditionManager.Instance.StartExpedition(id);
            RefreshUI();
            return;
        }

        // 2) 완료 상태 → 보상 수령(수동)
        if (ExpeditionManager.Instance.IsDone(id))
        {
            if (ExpeditionManager.Instance.TryClaimReward(id))
            {
                // 수령 후 상태 초기화됨 → UI 갱신
                RefreshUI();
            }
            return;
        }

        // 3) 진행 중이면 아무 것도 하지 않음
    }


    private void OnExpeditionChanged(string changedId)
    {
        if (runtimeSO == null) return;
        if (changedId != runtimeSO.indate) return;
        RefreshUI();
    }

    private void RefreshUI()
    {
        if (ExpeditionManager.Instance == null || runtimeSO == null) return;

        var id = runtimeSO.indate;
        if (string.IsNullOrEmpty(id))
        {
            if (mainBtn) mainBtn.interactable = false;
            if (btnText) btnText.text = "미설정";
            if (stateText) stateText.text = "데이터 없음";
            if (timerText) timerText.text = "";
            return;
        }

        // 명성도 체크
        int currentReputation = BackendGameData.Instance.userData.reputation;

        if (currentReputation < requiredReputation)
        {
            if (mainBtn) mainBtn.interactable = false;
            if (btnText) btnText.text = $"명성도\n{requiredReputation} 필요";
            if (stateText) stateText.text = "조건 미달";
            if (timerText) timerText.text = "";
            return;
        }

        bool isDone = ExpeditionManager.Instance.IsDone(id);
        if (isDone)
        {
            if (mainBtn) mainBtn.interactable = true;
            if (btnText) btnText.text = "보상 받기";
            if (stateText) stateText.text = "완료";
            if (timerText) timerText.text = "완료!";
            return; // ← 여기서 마무리
        }

        bool canStart = ExpeditionManager.Instance.CanStart(id);
        if (canStart)
        {
            if (mainBtn) mainBtn.interactable = true;
            if (btnText) btnText.text = "시작";
            if (stateText) stateText.text = "대기 중";
            if (timerText) timerText.text = "";
        }
        else
        {
            if (mainBtn) mainBtn.interactable = false;   // 진행 중에는 비활성
            if (btnText) btnText.text = "진행 중";
            if (stateText) stateText.text = "파견 중";
            // 타이머는 RefreshTimer에서 갱신
        }
    }

    private void RefreshTimer()
    {
        if (ExpeditionManager.Instance == null || runtimeSO == null) return;
        var id = runtimeSO.indate;
        if (string.IsNullOrEmpty(id)) return;

        bool isRunning = !ExpeditionManager.Instance.CanStart(id);
        if (!isRunning)
        {
            if (timerText != null) timerText.text = "";
            return;
        }

        var rem = ExpeditionManager.Instance.GetRemaining(id);

        if (rem.TotalSeconds <= 0)
        {
            // ★ 자동 지급 금지: 완료 UI만 표시
            if (timerText != null) timerText.text = "완료!";
            // 진행 중 플래그 해제 + UI 갱신은 IsDone/RefreshUI 경유
            ExpeditionManager.Instance.IsDone(id); // 진행 종료 처리(보상 미지급)
            RefreshUI();
            return;
        }

        if (timerText != null)
            timerText.text = $"{rem.Hours:D2}:{rem.Minutes:D2}:{rem.Seconds:D2}";
    }


    private void RefreshName()
    {
        if (nameText != null && runtimeSO != null && runtimeSO.staticSO != null)
            nameText.text = runtimeSO.staticSO.displayName;
    }
}
