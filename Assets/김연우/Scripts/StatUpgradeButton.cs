using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class StatUpgradeButton : MonoBehaviour
{
    public PlayerStatData playerStats;

    [Header("표시 UI")]

    public Sprite characterSprite;         // 표시할 캐릭터 스프라이트 (없으면 인스펙터에서 할당)
    public TMP_Text levelText;             // 레벨
    public TMP_Text attackText;            // 공격력
    public TMP_Text autoIntervalText;      // 자동공격속도
    public TMP_Text critChanceText;        // 크리티컬 확률
    public TMP_Text critDamageText;        // 크리티컬 데미지
    public TMP_Text moneyText;             // 가격
    public TMP_Text buttonText;            // 버튼 라벨 (Upgrade)
    private Button _button;

    // 가격 규칙 (원하면 바꿔)
    public int basePrice = 100;
    public float priceScale = 1.15f;  // 지수성장

    int GetCurrentPrice()
    {
        // 현재 level 기준 다음 한 단계 비용
        int L = Mathf.Max(playerStats.minLevel, playerStats.level);
        double cost = basePrice * System.Math.Pow(priceScale, L - 1);
        return Mathf.Max(1, Mathf.RoundToInt((float)cost));
    }

    void Awake()
    {
        Debug.Log($"[Awake] {name} 에서 리스너 등록", this);
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnClick);

    }
    void Start()
    {
        
        playerStats.basicAtk = BackendGameData.Instance.userData.basicAtk;
        playerStats.RecalculateFromBasicAtk();
        RefreshUI();
    }
    void OnEnable()
    {
        RefreshUI();
    }

    void OnClick()
    {
        Debug.Log($"[OnClick] {name} 클릭!", this);
        int price = GetCurrentPrice();
        if (BackendGameData.Instance.userData.gold < price)
        {
            Debug.Log("돈부족");
            return;
        }
        // ★ 네 프로젝트의 머니 차감 로직을 그대로 사용
        EventBus<MoneyChangeMusHandler>.Raise(new MoneyChangeMusHandler(price));

        // 레벨 +1 (== basicAtk 재산출 → 파생 전부 재계산)
        playerStats.ApplyLevelUp(1);


        BackendGameData.Instance.userData.basicAtk = playerStats.basicAtk;
        //       BackendGameData.Instance.SaveUserData();  // 네가 쓰는 저장 함수로 교체

        // 파생 바뀜 알림(필요한 UI들이 갱신하도록)
        EventBus<StatChangedEvent>.Raise(new StatChangedEvent { changedStatType = StatType.AttackPower });

        RefreshUI();
        Debug.Log($"[UPGRADE] L{playerStats.level} / ATK:{playerStats.attackPower}, " +
                  $"CR:{playerStats.critChance}%, INT:{playerStats.autoAttackInterval}s, CDMG:{playerStats.critDamageMultiplier}x");
    }

    void RefreshUI()
    {
        // 버튼 텍스트
        if (buttonText != null) buttonText.text = "강화";

        // 레벨
        if (levelText != null) levelText.text = $"Lv. {playerStats.level}";

        // 공격력
        if (attackText != null) attackText.text = $"공격력 : {playerStats.attackPower:F0}";

        // 자동공격속도 (초)
        if (autoIntervalText != null) autoIntervalText.text = $"자동공격속도 : {playerStats.autoAttackInterval:F2}s";

        // 크리티컬 확률 (%)
        if (critChanceText != null) critChanceText.text = $"크리티컬 확률 : {playerStats.critChance:F1}%";

        // 크리티컬 데미지 (배)
        if (critDamageText != null) critDamageText.text = $"크리티컬 데미지 : {playerStats.critDamageMultiplier:F2}배";

        // 가격
        if (moneyText != null) moneyText.text = $"필요 골드 : {GetCurrentPrice()}G";
    }
}