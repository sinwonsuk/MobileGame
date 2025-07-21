// == StaffShopButton.cs ==
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StaffShopButton1 : MonoBehaviour
{
    [Header("할당할 직원 데이터(SO)")]
    public StaffStatsSO staffData1;
    public Transform spawnPoint1;
    public Button purchaseButton1;
    StaffStatsSO staff1;
    StaffBase _spawnedStaff1;
    public TextMeshProUGUI _buttonText1;
    public TextMeshProUGUI level_num1;
    public StaffType staff13;
    void Awake()
    {
        if (spawnPoint1 == null)// 에디터에서 연결해두지 않았다면
        {
            
                var parent = GameObject.Find("GameObject (2)");
                if (parent != null)
                {
                    spawnPoint1 = parent.transform;
                }
                else
                {
                    Debug.LogWarning("MapParent가 없거나 자식이 없습니다.gaewe");
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
        if (_spawnedStaff1 == null)
        {
            if (spawnPoint1 != null)
            {
                // -- 첫 구매 --
                var go = Instantiate(staffData1.itemPrefab,
                                     spawnPoint1.position,
                                     spawnPoint1.rotation);
                _spawnedStaff1 = go.GetComponent<StaffBase>();
                _spawnedStaff1.Init(staffData1);
            }


            _buttonText1.text = "Upgrade";
            level_num1.text = $"Lv. {staffData1.level}";
        }
        else
        {
            // -- 레벨업(업그레이드) --
            _spawnedStaff1.LevelUp();
            level_num1.text = $"Lv. {staffData1.level}";
        }
    }
}
