using TMPro;
using UnityEngine;

public class StaffInfoUI : MonoBehaviour
{
    [Header("연결된 직원 데이터 (SO)")]
    public StaffStatsSO staffData;             // 정적 데이터 (이름)
    public RuntimeStaffStatsSO staffRuntime;   // 런타임 데이터 (레벨, 공격력, 공격속도)

    [Header("UI")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI atkText;
    public TextMeshProUGUI atkSpdText;

    private void OnEnable()
    {
        var em = EmployeeManager.Instance;
        if (em != null) em.OnStaffChanged += Refresh;

        Refresh();
    }

    private void OnDisable()
    {
        var em = EmployeeManager.Instance;
        if (em != null) em.OnStaffChanged -= Refresh;
    }

    private void Refresh()
    {
        if (staffData == null || staffRuntime == null) return;

        if (nameText != null)
            nameText.text = staffData.displayName;

        if (levelText != null)
            levelText.text = $"Lv. {staffRuntime.level}";

        if (atkText != null)
            atkText.text = $"공격력 : {staffRuntime.attack_Power:0}";

        if (atkSpdText != null)
            atkSpdText.text = $"공격속도 : {staffRuntime.attack_Speed:0.##}/s";
    }
}
