using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

[RequireComponent(typeof(Button))]
public class StatUpgradeButton : MonoBehaviour
{
    public StatType statToUpgrade;
    public float initialValue = 10f;
    public float upgradeAmount = 1f;
    public float minAutoAttackInterval = 0.1f;

    public TMP_Text nameText;
    public TMP_Text levelText;
    public TMP_Text valueText;
    public TMP_Text buttonText;
    public TMP_Text moneyText;
    private Button _button;
    private float currentValue;
    private int currentLevel;
    private int buy_money = 100;
    private int money_value =0;
    public PlayerStatData playerStats;

    // ★ 저장 키
    private string levelKey => $"{statToUpgrade}_level";
    private string valueKey => $"{statToUpgrade}_value";

    void Awake()
    {
        money_value = currentLevel * buy_money;
        _button = GetComponent<Button>();
        //PlayerPrefs.DeleteKey(levelKey);
        //PlayerPrefs.DeleteKey(valueKey);
    }

    void Start()
    {
        // 불러오기
        currentLevel = PlayerPrefs.GetInt(levelKey, 1);

        if (PlayerPrefs.HasKey(valueKey))
            currentValue = PlayerPrefs.GetFloat(valueKey);
        else
            currentValue = initialValue + upgradeAmount * currentLevel;

        _button.onClick.AddListener(OnClick);
        RefreshUI();
    }

    void OnClick()
    {
        money_value = currentLevel * buy_money;
        EventBus<MoneyChangeMusHandler>.Raise(new MoneyChangeMusHandler(money_value));
        currentLevel++;
        UpgradeStat();
        Save();
        RefreshUI();
        Debug.Log($"[StatUpgrade] {GetStatDisplayName(statToUpgrade)} leveled up to {currentLevel}. New value: {GetStatValue():F2}");
    }

    void RefreshUI()
    {
        nameText.text = GetStatDisplayName(statToUpgrade);
        levelText.text = $"Lv.{currentLevel}";
        valueText.text = FormatStatValue(GetStatValue());
        buttonText.text="Upgrade";
        moneyText.text = $"Money : {money_value}";
    }

    void UpgradeStat()
    {
        switch (statToUpgrade)
        {
            case StatType.AttackPower:
                currentValue += upgradeAmount;
                playerStats.attackPower = currentValue;
                break;
            case StatType.CritChance:
                currentValue = Mathf.Clamp(currentValue + upgradeAmount * 0.1f, 0f, 100f);
                playerStats.critChance = currentValue;
                break;
            case StatType.AutoAttackInterval:
                currentValue = Mathf.Max(minAutoAttackInterval, currentValue - upgradeAmount * 0.1f);
                playerStats.autoAttackInterval = currentValue;
                break;
            case StatType.AutoAttackDamage:
                currentValue += upgradeAmount;
                playerStats.autoAttackDamage = currentValue;
                break;
            case StatType.CritDamageMultiplier:
                currentValue += upgradeAmount * 0.1f;
                playerStats.critDamageMultiplier = currentValue;
                break;
        }
        EventBus<StatChangedEvent>.Raise(new StatChangedEvent { changedStatType = statToUpgrade });
    }

    // ★ 저장 메서드
    void Save()
    {
        PlayerPrefs.SetInt(levelKey, currentLevel);
        PlayerPrefs.SetFloat(valueKey, currentValue);
        PlayerPrefs.Save();
    }

    float GetStatValue() => currentValue;

    string FormatStatValue(float value)
    {
        switch (statToUpgrade)
        {
            case StatType.CritChance: return $"{value:F1}%";
            case StatType.AutoAttackInterval: return $"{value:F2}s";
            case StatType.CritDamageMultiplier: return $"{value:F1}배";
            default: return $"{value:F0}";
        }
    }

    string GetStatDisplayName(StatType type)
    {
        switch (type)
        {
            case StatType.AttackPower: return "attack";
            case StatType.CritChance: return "critical";
            case StatType.AutoAttackInterval: return "auto attack time";
            case StatType.AutoAttackDamage: return "auto attack damage";
            case StatType.CritDamageMultiplier: return "critical damage";
            default: return string.Empty;
        }
    }
}
