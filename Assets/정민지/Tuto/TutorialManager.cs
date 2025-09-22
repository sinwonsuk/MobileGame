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
    public TutorialData shopTutorial;
    private TutorialData tutorialData;

    private int currentStepIndex = 0;
    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private bool isTutorialPlaying = false;
    private bool waitingForButtonClick = false;
    private Transform originalPointerParent;

    private Dictionary<string, Button> buttonMap = new Dictionary<string, Button>();
    public static TutorialManager Instance;

    private TutorialType currentTutorialType; // 현재 튜토리얼 타입 저장

    public enum TutorialType
    {
        Start,
        Staff,
        Hunter,
        Inventory,
        HunterShop,
        FoodLevelUp,
        Shop
    }

    void Awake()
    {
        if (Instance == null) Instance = this;
        originalPointerParent = handPointer.parent;
    }

    void Start()
    {
        if (startTutorial != null&&!tutorialBool.Instance.clearStartTuto)
        {
            StartTutorial(TutorialType.Start);
        }
        else
        {
            dialoguePanel.SetActive(false);
            dialogueBackground.gameObject.SetActive(false);
            leftCharacterImage.gameObject.SetActive(false);
            rightCharacterImage.gameObject.SetActive(false);
            handPointer.gameObject.SetActive(false);
            QImage.gameObject.SetActive(false);
            dialogueText.text = "";
            questText.text = "";
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

        currentTutorialType = type;

        tutorialData = type switch
        {
            TutorialType.Start => startTutorial,
            TutorialType.Staff => staffTutorial,
            TutorialType.Hunter => hunterTutorial,
            TutorialType.Inventory => inventoryTutorial,
            TutorialType.HunterShop => hunterShopTutorial,
            TutorialType.FoodLevelUp => foodLevelUpTutorial,
            TutorialType.Shop => shopTutorial,
            _ => null
        };

        if (tutorialData == null)
        {
            Debug.LogWarning("[TutorialManager] 해당 튜토리얼 데이터 없음");
            return;
        }

        ShowStep(0);
        isTutorialPlaying = true;
    }

    public void OnDialoguePanelClicked()
    {
        if (!isTutorialPlaying) return;

        var step = tutorialData.steps[currentStepIndex];

        if (step.isButtonStep || step.trigger == StepTrigger.OnEvent) return;

        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            dialogueText.maxVisibleCharacters = dialogueText.text.Length;
            isTyping = false;
        }
        else
        {
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
            SetupButtonStep(step);
            return;
        }

        // -------------------
        // 이벤트 단계
        // -------------------
        if (step.trigger == StepTrigger.OnEvent)
        {
            SetupEventStep(step);
            return;
        }

        // -------------------
        // 대사 단계
        // -------------------
        SetupDialogueStep(step);
    }

    private void SetupButtonStep(TutorialStep step)
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
    }

    private void SetupEventStep(TutorialStep step)
    {
        waitingForButtonClick = false;

        dialoguePanel.SetActive(false);
        QImage.SetActive(true);
        questText.gameObject.SetActive(true);
        dialogueBackground.gameObject.SetActive(false);
        handPointer.gameObject.SetActive(false);

        dialogueText.text = "";
        questText.text = step.questText ?? "";
    }

    private void SetupDialogueStep(TutorialStep step)
    {
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

        pointerRect.SetParent(btnRect, false);
        pointerRect.anchorMin = new Vector2(1, 0.5f);
        pointerRect.anchorMax = new Vector2(1, 0.5f);
        pointerRect.pivot = new Vector2(0f, 0.5f);
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
                pointerRect.SetParent(originalPointerParent, false);
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
        QImage.gameObject.SetActive(false);
        dialogueText.text = "";
        questText.text = "";

        isTutorialPlaying = false;
        Debug.Log("튜토리얼 종료!");

        switch (currentTutorialType)
        {
            case TutorialType.Start:
                tutorialBool.Instance.clearStartTuto = true;
                break;
            case TutorialType.Staff:
                tutorialBool.Instance.clearBuyStaffTuto = true;
                break;
            case TutorialType.Hunter:
                tutorialBool.Instance.clearBuyHunterTuto = true;
                break;
            case TutorialType.Inventory:
                tutorialBool.Instance.clearInvenTuto = true;
                break;
            case TutorialType.HunterShop:
                tutorialBool.Instance.clearDispatchTuto = true;
                break;
            case TutorialType.FoodLevelUp:
                tutorialBool.Instance.clearLevelUpTuto = true;
                break;
            case TutorialType.Shop:
                tutorialBool.Instance.clearShopTuto = true;
                break;
        }

        TutorialInit.Instance?.SaveTuto();
    }

    public void TriggerEvent(string eventName)
    {
        if (!isTutorialPlaying) return;

        var step = tutorialData.steps[currentStepIndex];

        if (step.trigger == StepTrigger.OnEvent && step.eventName == eventName)
        {
            Debug.Log("[TriggerEvent] 조건 일치 → 다음 스텝으로 이동");

            if (eventName == "killMonster")
            {
                InventoryManager.Instance?.AddItem("2025-07-16T02:27:43.737Z", 1);
                InventoryManager.Instance?.AddItem("2025-07-16T02:27:43.883Z", 1);
            }

            NextStep();
        }
    }

    //테스트용 함수
    public void Reset()
    {
        tutorialBool.Instance.clearStartTuto = false;
        tutorialBool.Instance.clearBuyHunterTuto = false;
        tutorialBool.Instance.clearBuyStaffTuto = false;
        tutorialBool.Instance.clearInvenTuto = false;
        tutorialBool.Instance.clearDispatchTuto = false;
        tutorialBool.Instance.clearLevelUpTuto = false;
        tutorialBool.Instance.clearShopTuto = false;

        TutorialInit.Instance?.SaveTuto();
    }

    public void TestHunter()
    {
        if (!tutorialBool.Instance.clearBuyHunterTuto)
        {
            StartTutorial(TutorialType.Hunter);
        }
    }

    public void TestStaff()
    {
        if (!tutorialBool.Instance.clearBuyStaffTuto)
        {
            StartTutorial(TutorialType.Staff);
        }
    }

    public void TestInterior()
    {
        if (!tutorialBool.Instance.clearShopTuto)
            StartTutorial(TutorialType.Shop);
    }
}
