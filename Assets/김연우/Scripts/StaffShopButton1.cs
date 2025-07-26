// == StaffShopButton.cs ==
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StaffShopButton1 : MonoBehaviour
{
    [Header("�Ҵ��� ���� ������(SO)")]
    public StaffStatsSO staffData1;
    public RuntimeStaffStatsSO RuntimestaffData1;
    public Transform spawnPoint1;
    public Button purchaseButton1;
    StaffStatsSO staff1;
    StaffBase _spawnedStaff1;
    public TextMeshProUGUI _buttonText1;
    public TextMeshProUGUI level_num1;
    public StaffType staff13;
    private int money;
    void Awake()
    {
        money = staffData1.baseSalary;
        if (spawnPoint1 == null)// �����Ϳ��� �����ص��� �ʾҴٸ�
        {

            var parent1 = GameObject.Find("MapPoint");
            if (parent1 != null && parent1.transform.childCount > 0)
            {
                spawnPoint1 = parent1.transform.GetChild(4);
            }




        }
    }
    void Start()
    {
        level_num1.text = $"Lv. 0";
        purchaseButton1.onClick.AddListener(OnButtonClicked);
    }

    void OnButtonClicked()
    {
        EventBus<MoneyChangeMusHandler>.Raise(new MoneyChangeMusHandler(money));
        money = staffData1.baseSalary * RuntimestaffData1.level;
        if (_spawnedStaff1 == null)
        {

            if (spawnPoint1 != null)
            {
                // -- ù ���� --
                var go = Instantiate(staffData1.itemPrefab,
                                     spawnPoint1.position,
                                     spawnPoint1.rotation);
                _spawnedStaff1 = go.GetComponent<StaffBase>();
                _spawnedStaff1.Init(staffData1, RuntimestaffData1);
            }


            _buttonText1.text = "Upgrade";
            level_num1.text = $"Lv. {RuntimestaffData1.level}";
        }
        else
        {
            // -- ������(���׷��̵�) --
            _spawnedStaff1.LevelUp();
            level_num1.text = $"Lv. {RuntimestaffData1.level}";
        }
    }
}
