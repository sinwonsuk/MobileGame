// StatUpgradeButton.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class StatUpgradeButton : MonoBehaviour
{
    [Header("Settings")]
    public StatType statToUpgrade;
    public float initialValue = 10f;           // ó�� ��ġ (Inspector�� ����)
    public float upgradeAmount = 1f;           // ���׷��̵� ����
    public float minAutoAttackInterval = 0.1f; // �ڵ����� �ּ� ���� (���� ���� ��)

    [Header("UI References")]
    public TMP_Text nameText;
    public TMP_Text levelText;
    public TMP_Text valueText;
    public TMP_Text buttonText;

    private Button _button;
    private float currentValue;
    private int currentLevel;

    void Awake()
    {
        _button = GetComponent<Button>();
    }

    void Start()
    {
        // �ʱⰪ ����
        currentValue = initialValue;
        currentLevel = 0;

        // ��ư Ŭ�� �ڵ鷯
        _button.onClick.AddListener(() =>
        {
            currentLevel++;
            UpgradeStat();
            RefreshUI();
            Debug.Log($"[StatUpgrade] {GetStatDisplayName(statToUpgrade)} leveled up to {currentLevel}. New value: {GetStatValue():F2}");
        });

        RefreshUI();
    }

    void RefreshUI()
    {
        nameText.text = GetStatDisplayName(statToUpgrade);
        levelText.text = $"Lv.{currentLevel}";
        valueText.text = FormatStatValue(GetStatValue());
        buttonText.text = currentLevel == 0 ? "Buy" : "Upgrade";
    }

    void UpgradeStat()
    {
        switch (statToUpgrade)
        {
            case StatType.AttackPower:
                currentValue += upgradeAmount;
                break;
            case StatType.CritChance:
                // 0.01% ������ ����, �ִ� 100%
                currentValue = Mathf.Clamp(currentValue + upgradeAmount * 0.1f, 0f, 100f);
                break;
            case StatType.AutoAttackInterval:
                currentValue = Mathf.Max(minAutoAttackInterval, currentValue - upgradeAmount * 0.1f);
                break;
            case StatType.AutoAttackDamage:
                currentValue += upgradeAmount;
                break;
            case StatType.CritDamageMultiplier:
                currentValue += upgradeAmount * 0.1f;
                break;
        }
    }

    float GetStatValue()
    {
        return currentValue;
    }

    string FormatStatValue(float value)
    {
        switch (statToUpgrade)
        {
            case StatType.CritChance:
                return $"{value:F1}%";
            case StatType.AutoAttackInterval:
                return $"{value:F2}s";
            case StatType.CritDamageMultiplier:
                return $"{currentValue:F1}��";
            default:
                return $"{value:F0}";
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
