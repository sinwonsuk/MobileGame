using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StaffShopButton : MonoBehaviour
{
    [Header("�Ҵ��� ���� ������(SO)")]
    public StaffStatsSO staffData;
    public RuntimeStaffStatsSO RuntimeStaffData;
    public Transform spawnPoint;
    public Button purchaseButton;
    StaffStatsSO staff;
    StaffBase _spawnedStaff;
    public TextMeshProUGUI _buttonText;
    public TextMeshProUGUI level_num;
    public StaffType staff123;
    public string num1;
    public int money;
    void Awake()
    {
        money = staffData.baseSalary;
        if (spawnPoint == null)// �����Ϳ��� �����ص��� �ʾҴٸ�
        {
            var parent = GameObject.Find("MapPoint");
            switch (num1)
            {
                case "first":
                    if (parent != null && parent.transform.childCount > 0)
                    {
                        spawnPoint = parent.transform.GetChild(0);
                    }
                    else
                    {
                        Debug.LogWarning("MapParent�� ���ų� �ڽ��� �����ϴ�rrrr.");
                    }
                    break;
                case "second":
                    if (parent != null && parent.transform.childCount > 0)
                    {
                        spawnPoint = parent.transform.GetChild(1);
                    }
                    else
                    {
                        Debug.LogWarning("MapParent�� ���ų� �ڽ��� �����ϴ�rrrr.");
                    }
                    break;
                case "third":
                    if (parent != null && parent.transform.childCount > 0)
                    {
                        spawnPoint = parent.transform.GetChild(2);
                    }
                    else
                    {
                        Debug.LogWarning("MapParent�� ���ų� �ڽ��� �����ϴ�rrrr.");
                    }
                    break;
                case "forth":
                    if (parent != null && parent.transform.childCount > 0)
                    {
                       spawnPoint = parent.transform.GetChild(3);
                    }
                    else
                    {
                        Debug.LogWarning("MapParent�� ���ų� �ڽ��� �����ϴ�rrrr.");
                    }
                    break;

            }


           
        }
    }
    void Start()
    {
        level_num.text = $"Lv. 0";
        purchaseButton.onClick.AddListener(OnButtonClicked);
    }

    void OnButtonClicked()
    {
        EventBus<MoneyChangeMusHandler>.Raise(new MoneyChangeMusHandler(money));
        money = staffData.baseSalary * RuntimeStaffData.level;
        if (_spawnedStaff == null)
        {
            if (spawnPoint != null)
            {
                // -- ù ���� --
                var go = Instantiate(staffData.itemPrefab,
                                     spawnPoint.position,
                                     spawnPoint.rotation);
                _spawnedStaff = go.GetComponent<StaffBase>();
                _spawnedStaff.Init(staffData, RuntimeStaffData);
            }


            _buttonText.text = "Upgrade";
            level_num.text = $"Lv. {RuntimeStaffData.level}";
        }
        else
        {
            // -- ������(���׷��̵�) --
            _spawnedStaff.LevelUp();
            level_num.text = $"Lv. {RuntimeStaffData.level}";
        }
    }
}
/*using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StaffShopButton : MonoBehaviour
{
    [Header("�Ҵ��� ���� ������(SO)")]
    public StaffStatsSO staffData;
    public Transform spawnPoint;
    public Button purchaseButton;
    StaffStatsSO staff;
    StaffBase _spawnedStaff;
    public TextMeshProUGUI _buttonText;
    public TextMeshProUGUI level_num;
    public StaffType staff123;
    void Awake()
    {
        if (spawnPoint == null)// �����Ϳ��� �����ص��� �ʾҴٸ�
        {
            if (staff123 == StaffType.hunter)
            {
                var parent = GameObject.Find("MapParent");
                if (parent != null && parent.transform.childCount > 0)
                {
                    spawnPoint = parent.transform.GetChild(0);
                }
                else
                {
                    Debug.LogWarning("MapParent�� ���ų� �ڽ��� �����ϴ�rrrr.");
                }
            }
            else if (staff123 == StaffType.restaurant)
            {
                var parent = GameObject.Find("GameObject (2)");
                if (parent != null)
                {
                    spawnPoint = parent.transform;
                }
                else
                {
                    Debug.LogWarning("MapParent�� ���ų� �ڽ��� �����ϴ�.gaewe");
                }

            }


        }
    }
    void Start()
    {
        level_num.text = $"Lv. 0";
        purchaseButton.onClick.AddListener(OnButtonClicked);
    }

    void OnButtonClicked()
    {
        if (_spawnedStaff == null)
        {
            if (spawnPoint != null)
            {
                // -- ù ���� --
                var go = Instantiate(staffData.itemPrefab,
                                     spawnPoint.position,
                                     spawnPoint.rotation);
                _spawnedStaff = go.GetComponent<StaffBase>();
                _spawnedStaff.Init(staffData);
            }


            _buttonText.text = "Upgrade";
            level_num.text = $"Lv. {staffData.level}";
        }
        else
        {
            // -- ������(���׷��̵�) --
            _spawnedStaff.LevelUp();
            level_num.text = $"Lv. {staffData.level}";
        }
    }
}
*/