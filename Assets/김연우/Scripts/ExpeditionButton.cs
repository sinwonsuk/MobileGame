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

        //  1) 완료 상태면 '보상 지급' 먼저 처리
        if (ExpeditionManager.Instance.IsDone(id))
        {
            if (ExpeditionManager.Instance.TryClaimReward(id))  // ← 여기서만 지급!
            {
                RefreshUI(); // 보상 후 '출발' 상태로 돌아가게
            }
            return;
        }

        // ) 그 다음에 출발 가능 여부 체크
        if (ExpeditionManager.Instance.CanStart(id))
        {
            ExpeditionManager.Instance.StartExpedition(id);
            RefreshUI();
            return;
        }

        // 진행 중이면 무시
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
            return;
        }

        bool canStart = ExpeditionManager.Instance.CanStart(id);
        if (canStart)
        {
            if (mainBtn) mainBtn.interactable = true;
            if (btnText) btnText.text = "출발";
            if (stateText) stateText.text = "대기 중";
            if (timerText) timerText.text = "";
        }
        else
        {
            if (mainBtn) mainBtn.interactable = false;
            if (btnText) btnText.text = "파견중";
            if (stateText) stateText.text = "진행 중";
            // 타이머는 RefreshTimer에서 갱신
        }
    }



    // ExpeditionButton.cs
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
            //  자동 수령 금지: 완료 상태만 만들고 끝
            if (timerText != null) timerText.text = "완료!";
            ExpeditionManager.Instance.IsDone(id); // 진행만 종료
            RefreshUI();                           // 버튼을 "보상 받기"로
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
