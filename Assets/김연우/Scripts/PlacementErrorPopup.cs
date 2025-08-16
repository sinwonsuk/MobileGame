using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 이벤트 페이로드
public struct ShowPlacementErrorPopup : IEvent
{
    public string message;
    public Action onOk;
    public ShowPlacementErrorPopup(string message, Action onOk)
    {
        this.message = message;
        this.onOk = onOk;
    }
}

public class PlacementErrorPopup : MonoBehaviour
{
    [Header("Popup Root (비활성으로 시작)")]
    public GameObject root;          // Panel_Root 같은 실제 팝업 패널 오브젝트
    public TMP_Text messageText;     // "전투직원이 아닙니다" 같은 문구
    public Button okButton;

    private void Awake()
    {
        if (root != null) root.SetActive(false);
        EventBus<ShowPlacementErrorPopup>.OnEvent += OnShowPopup;
    }

    private void OnDestroy()
    {
        EventBus<ShowPlacementErrorPopup>.OnEvent -= OnShowPopup;
    }

    private void OnShowPopup(ShowPlacementErrorPopup e)
    {
        if (messageText != null) messageText.text = e.message ?? "";
        if (okButton != null)
        {
            okButton.onClick.RemoveAllListeners();
            okButton.onClick.AddListener(() =>
            {
                root?.SetActive(false);
                e.onOk?.Invoke(); // 확인 시 화살표 제거 등 후처리
            });
        }
        root?.SetActive(true);
    }
}
