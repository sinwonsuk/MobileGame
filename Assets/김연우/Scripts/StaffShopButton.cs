using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StaffShopButton : MonoBehaviour
{
    [Header("할당할 직원 데이터(SO)")]
    public StaffStatsSO staffData;
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
        if (spawnPoint == null)// 에디터에서 연결해두지 않았다면
        {
            var parent = GameObject.Find("MapParent");
            switch (num1)
            {
                case "first":
                    if (parent != null && parent.transform.childCount > 0)
                    {
                        spawnPoint = parent.transform.GetChild(0);
                    }
                    else
                    {
                        Debug.LogWarning("MapParent가 없거나 자식이 없습니다rrrr.");
                    }
                    break;
                case "second":
                    if (parent != null && parent.transform.childCount > 0)
                    {
                        spawnPoint = parent.transform.GetChild(1);
                    }
                    else
                    {
                        Debug.LogWarning("MapParent가 없거나 자식이 없습니다rrrr.");
                    }
                    break;
                case "third":
                    if (parent != null && parent.transform.childCount > 0)
                    {
                        spawnPoint = parent.transform.GetChild(2);
                    }
                    else
                    {
                        Debug.LogWarning("MapParent가 없거나 자식이 없습니다rrrr.");
                    }
                    break;
                case "forth":
                    if (parent != null && parent.transform.childCount > 0)
                    {
                       spawnPoint = parent.transform.GetChild(3);
                    }
                    else
                    {
                        Debug.LogWarning("MapParent가 없거나 자식이 없습니다rrrr.");
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
        money = staffData.baseSalary * staffData.level;
        if (_spawnedStaff == null)
        {
            if (spawnPoint != null)
            {
                // -- 첫 구매 --
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
            // -- 레벨업(업그레이드) --
            _spawnedStaff.LevelUp();
            level_num.text = $"Lv. {staffData.level}";
        }
    }
}
/*using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StaffShopButton : MonoBehaviour
{
    [Header("할당할 직원 데이터(SO)")]
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
        if (spawnPoint == null)// 에디터에서 연결해두지 않았다면
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
                    Debug.LogWarning("MapParent가 없거나 자식이 없습니다rrrr.");
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
                    Debug.LogWarning("MapParent가 없거나 자식이 없습니다.gaewe");
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
                // -- 첫 구매 --
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
            // -- 레벨업(업그레이드) --
            _spawnedStaff.LevelUp();
            level_num.text = $"Lv. {staffData.level}";
        }
    }
}
*/