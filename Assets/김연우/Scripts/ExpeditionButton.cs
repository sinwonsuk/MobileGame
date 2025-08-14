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


    public Action<string, Action> OnRequestClaim;

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

        if (ExpeditionManager.Instance.CanStart(id))
        {
            ExpeditionManager.Instance.StartExpedition(id);
        }
        else if (ExpeditionManager.Instance.IsDone(id))
        {

            ConfirmClaim();
        }
    }
public void ConfirmClaim()
{
    if (ExpeditionManager.Instance == null || runtimeSO == null) return;
    var id = runtimeSO.indate;
    if (string.IsNullOrEmpty(id)) return;

        ExpeditionManager.Instance.TryClaimReward(id);
    RefreshUI();
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

        bool canStart = ExpeditionManager.Instance.CanStart(id);
        bool isDone = ExpeditionManager.Instance.IsDone(id);  

        if (canStart)
        {
            if (mainBtn) mainBtn.interactable = true;
            if (btnText) btnText.text = "시작";
            if (stateText) stateText.text = "대기 중";
            if (timerText) timerText.text = "";
        }
        else if (isDone)
        {

            if (mainBtn) mainBtn.interactable = false;
            if (btnText) btnText.text = "완료";
            if (stateText) stateText.text = "완료";
        }
        else
        {
            if (mainBtn) mainBtn.interactable = false;
            if (btnText) btnText.text = "진행 중";
            if (stateText) stateText.text = "파견 중";
        }
    }

    private void RefreshTimer()
    {
        if (ExpeditionManager.Instance == null || runtimeSO == null) return;
        var id = runtimeSO.indate;
        if (string.IsNullOrEmpty(id)) return;

        var rem = ExpeditionManager.Instance.GetRemaining(id);  
        bool isRunning = !ExpeditionManager.Instance.CanStart(id);

        if (timerText != null)
        {
            if (isRunning && rem.TotalSeconds > 0)
                timerText.text = $"{rem.Hours:D2}:{rem.Minutes:D2}:{rem.Seconds:D2}";
            else
                timerText.text = isRunning ? "완료!" : "";
        }
    }

    private void RefreshName()
    {
        if (nameText != null && runtimeSO != null && runtimeSO.staticSO != null)
            nameText.text = runtimeSO.staticSO.displayName;     
    }
}
