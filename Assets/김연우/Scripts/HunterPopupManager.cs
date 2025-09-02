using UnityEngine;
using UnityEngine.UI;

public class HunterPopupManager : MonoBehaviour
{
    [Header("열기 버튼들 (리스트/상점 쪽)")]
    public Button[] openButtons;    // 팝업을 열 버튼 배열

    [Header("팝업창 루트들")]
    public GameObject[] popups;     // HunterStaffBackGround (1~n)

    [Header("팝업 안의 X 버튼들")]
    public Button[] closeButtons;   // 각 팝업 안 닫기 버튼들

    private void Start()
    {
        // 시작은 전부 끄기
        HideAll();

        // 열기 버튼 바인딩
        for (int i = 0; i < openButtons.Length; i++)
        {
            int idx = i;
            if (openButtons[i] == null) continue;
            openButtons[i].onClick.AddListener(() => ShowPopup(idx));
        }

        // 닫기 버튼 바인딩
        for (int i = 0; i < closeButtons.Length; i++)
        {
            int idx = i;
            if (closeButtons[i] == null) continue;
            closeButtons[i].onClick.AddListener(() => HidePopup(idx));
        }
    }

    // 특정 팝업 열기
    private void ShowPopup(int index)
    {
        HideAll(); // 중복 방지
        if (!IsValid(index)) return;

        popups[index].transform.SetAsLastSibling(); // 맨 위로 올리기
        popups[index].SetActive(true);

        // 클릭 사운드
        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click, false);
    }

    // 특정 팝업 닫기
    private void HidePopup(int index)
    {
        if (!IsValid(index)) return;
        popups[index].SetActive(false);

         SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click, false);
    }

    // 전부 닫기
    private void HideAll()
    {
        foreach (var popup in popups)
        {
            if (popup != null) popup.SetActive(false);
        }
    }

    // 인덱스 유효성 체크
    private bool IsValid(int index)
    {
        return popups != null && index >= 0 && index < popups.Length && popups[index] != null;
    }
}
