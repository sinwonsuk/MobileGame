using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class TutorialManager : MonoBehaviour
{
    [Header("튜토리얼 UI 오브젝트")]
    public Image leftCharacterImage;
    public Image rightCharacterImage;
    public GameObject dialoguePanel;
    public Image dialogueBackground;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI questText;
    public TextMeshProUGUI nameText;
    public RectTransform handPointer;
    public GameObject QImage;

    [Header("튜토리얼 스크립터블 오브젝트")]
    public TutorialData startTutorial;
    public TutorialData staffTutorial;
    public TutorialData hunterTutorial;
    public TutorialData inventoryTutorial;
    public TutorialData hunterShopTutorial;
    public TutorialData foodLevelUpTutorial;
    public TutorialData interiorTutorial;
    public TutorialData shopTutorial;
    private TutorialData tutorialData;

    private int currentStepIndex = 0;
    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private bool isTutorialPlaying = false;
    private bool waitingForButtonClick = false;
    private Transform originalPointerParent;

    [Header("튜토리얼 완료 여부 bool값")]
    public bool clearStartTuto = false; //시작 튜토
    public bool clearInvenTuto = false; //인벤 튜토
    public bool clearBuyStaffTuto = false; //경영직원 구매 튜토
    public bool clearBuyHunterTuto = false; //헌터 구매 튜토
    public bool clearShopTuto = false; //상점 튜토
    public bool clearDispatchTuto = false; //강화 및 파견지 튜토
    public bool clearLevelUpTuto = false; //요리 강화 튜토

    private Dictionary<string, Button> buttonMap = new Dictionary<string, Button>();
    public static TutorialManager Instance;

    public enum TutorialType
    {
        Start,
        Staff,
        Hunter,
        Inventory,
        HunterShop,
        FoodLevelUp,
        Interior,
        Shop
    }

    void Awake()
    {
        if (Instance == null) Instance = this;
        originalPointerParent = handPointer.parent; // 최초 부모 기억
    }

    void Start()
    {
        if (startTutorial != null)
        {
            StartTutorial(TutorialType.Start);
        }
            
    }

    public void RegisterButton(string name, Button btn)
    {
        if (!buttonMap.ContainsKey(name))
            buttonMap[name] = btn;
    }

    public void StartTutorial(TutorialType type)
    {
        if (isTutorialPlaying) return;

        switch (type)
        {
            case TutorialType.Start:
                tutorialData = startTutorial;
                break;
            case TutorialType.Staff:
                tutorialData = staffTutorial;
                break;
            case TutorialType.Hunter:
                tutorialData = hunterTutorial;
                break;
            case TutorialType.Inventory:
                tutorialData = inventoryTutorial;
                break;
            case TutorialType.HunterShop:
                tutorialData = hunterShopTutorial;
                break;
            case TutorialType.FoodLevelUp:
                tutorialData = foodLevelUpTutorial;
                break;
            case TutorialType.Interior:
                tutorialData = interiorTutorial;
                break;
            case TutorialType.Shop:
                tutorialData = shopTutorial;
                break;
            default:
                Debug.LogWarning($"[TutorialManager] 알 수 없는 튜토리얼 타입: {type}");
                return;
        }

        ShowStep(0);
        isTutorialPlaying = true;
    }

    // ------------------------------
    // 대사창 클릭 전용
    // ------------------------------
    public void OnDialoguePanelClicked()
    {
        if (!isTutorialPlaying) return;

        var step = tutorialData.steps[currentStepIndex];

        // 버튼 단계와 이벤트 단계는 클릭 무시
        if (step.isButtonStep || step.trigger == StepTrigger.OnEvent) return;

        if (isTyping)
        {
            // 타이핑 중 → 한 번에 표시
            StopCoroutine(typingCoroutine);
            dialogueText.maxVisibleCharacters = dialogueText.text.Length;
            isTyping = false;
        }
        else
        {
            // 다 출력된 상태 → 다음 단계
            NextStep();
        }
    }

    public void ShowStep(int index)
    {
        if (index >= tutorialData.steps.Length)
        {
            EndTutorial();
            return;
        }

        currentStepIndex = index;
        var step = tutorialData.steps[index];

        // -------------------
        // 버튼 단계
        // -------------------
        if (step.isButtonStep)
        {
            waitingForButtonClick = true;

            dialoguePanel.SetActive(false);
            QImage.SetActive(true);
            questText.gameObject.SetActive(true);
            dialogueBackground.gameObject.SetActive(false);
            handPointer.gameObject.SetActive(true);

            dialogueText.text = "";
            questText.text = step.questText ?? "";

            if (!string.IsNullOrEmpty(step.targetButtonName))
            {
                if (buttonMap.TryGetValue(step.targetButtonName, out Button btn))
                    ConnectButton(btn, step.pointerOffset);
                else
                    StartCoroutine(WaitForButton(step.targetButtonName, step.pointerOffset));
            }

            return;
        }

        // -------------------
        // 이벤트 단계
        // -------------------
        if (step.trigger == StepTrigger.OnEvent)
        {
            waitingForButtonClick = false;

            dialoguePanel.SetActive(false);
            QImage.SetActive(true);
            questText.gameObject.SetActive(true);
            dialogueBackground.gameObject.SetActive(false);
            handPointer.gameObject.SetActive(false);

            dialogueText.text = "";
            questText.text = step.questText ?? "";

            return; // 입력에 의한 진행 막음
        }

        // -------------------
        // 대사 단계
        // -------------------
        waitingForButtonClick = false;

        dialoguePanel.SetActive(true);
        dialogueBackground.gameObject.SetActive(true);
        questText.gameObject.SetActive(false);
        QImage.SetActive(false);
        handPointer.gameObject.SetActive(false);

        nameText.text = step.characterName;
        nameText.color = step.characterNameColor;

        if (step.isLeftCharacter)
        {
            leftCharacterImage.sprite = step.characterSprite;
            leftCharacterImage.gameObject.SetActive(true);
            rightCharacterImage.gameObject.SetActive(false);
        }
        else
        {
            rightCharacterImage.sprite = step.characterSprite;
            rightCharacterImage.gameObject.SetActive(true);
            leftCharacterImage.gameObject.SetActive(false);
        }

        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeText(step.dialogue));
    }

    private void ConnectButton(Button btn, Vector2 pointerOffset)
    {
        RectTransform btnRect = btn.GetComponent<RectTransform>();
        RectTransform pointerRect = handPointer;

        // 핸드포인터를 버튼의 자식으로 붙여서 앵커를 따라가게
        pointerRect.SetParent(btnRect, worldPositionStays: false);

        // 원하는 앵커 기준으로 맞추기
        pointerRect.anchorMin = new Vector2(1, 0.5f); // 오른쪽 중앙
        pointerRect.anchorMax = new Vector2(1, 0.5f);
        pointerRect.pivot = new Vector2(0f, 0.5f);   // 왼쪽 중심 맞추기

        // 앵커를 기준으로 offset 조정
        pointerRect.anchoredPosition = pointerOffset;

        pointerRect.gameObject.SetActive(true);

        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() =>
        {
            var step = tutorialData.steps[currentStepIndex];
            if (!string.IsNullOrEmpty(step.targetButtonName) && step.targetButtonName == btn.name)
            {
                waitingForButtonClick = false;
                pointerRect.gameObject.SetActive(false);

                // 다시 원래 부모로 돌려주기 (예: UI Panel)
                pointerRect.SetParent(originalPointerParent, worldPositionStays: false);

                NextStep();
            }
        });
    }


    private IEnumerator WaitForButton(string buttonName, Vector2 pointerOffset)
    {
        Button targetBtn = null;
        while (!buttonMap.TryGetValue(buttonName, out targetBtn))
            yield return null;

        ConnectButton(targetBtn, pointerOffset);
    }

    private IEnumerator TypeText(string text)
    {
        isTyping = true;
        dialogueText.text = text;
        dialogueText.maxVisibleCharacters = 0;

        for (int i = 0; i <= text.Length; i++)
        {
            dialogueText.maxVisibleCharacters = i;
            yield return new WaitForSecondsRealtime(0.03f);
        }

        isTyping = false;
    }

    public void NextStep()
    {
        ShowStep(currentStepIndex + 1);
    }

    void EndTutorial()
    {
        dialoguePanel.SetActive(false);
        dialogueBackground.gameObject.SetActive(false);
        leftCharacterImage.gameObject.SetActive(false);
        rightCharacterImage.gameObject.SetActive(false);
        handPointer.gameObject.SetActive(false);
        dialogueText.text = "";
        questText.text = "";

        isTutorialPlaying = false;
        Debug.Log("튜토리얼 종료!");
    }

    public void TriggerEvent(string eventName)
    {
        if (!isTutorialPlaying) return;

        var step = tutorialData.steps[currentStepIndex];

        if (step.trigger == StepTrigger.OnEvent && step.eventName == eventName)
        {
            Debug.Log("[TriggerEvent] 조건 일치 → 다음 스텝으로 이동");
            if(eventName== "killMonster")
            {
                InventoryManager.Instance?.AddItem("2025-07-16T02:27:43.737Z", 1);
                InventoryManager.Instance?.AddItem("2025-07-16T02:27:43.883Z", 1);
            }
            NextStep();
        }
    }
}
