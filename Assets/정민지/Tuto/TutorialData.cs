using UnityEngine;

[CreateAssetMenu(fileName = "TutorialDataFinal", menuName = "Tutorial/TutorialDataFinal")]
public class TutorialData : ScriptableObject
{
    public TutorialStep[] steps;
}

[System.Serializable]
public class TutorialStep
{
    [TextArea] public string dialogue;      // 캐릭터 대사
    [TextArea] public string questText;     // 버튼/이벤트 단계 안내

    public string characterName;
    public Color characterNameColor = Color.white;

    public Sprite characterSprite;
    public bool isLeftCharacter;

    public bool isButtonStep;
    public string targetButtonName;
    public Vector2 pointerOffset;

    public StepTrigger trigger = StepTrigger.OnClick;
    public string eventName;
}

public enum StepTrigger
{
    OnClick,        // 화면 터치
    OnButtonClick,  // 버튼 클릭
    OnEvent         // 외부 이벤트
}
