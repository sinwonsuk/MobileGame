// == StaffShopButton.cs ==
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StaffShopButton : MonoBehaviour
{
    [Header("할당할 직원 데이터(SO)")]
    public StaffStatsSO staffData;
    public Transform spawnPoint;
    public Button purchaseButton;

    StaffBase _spawnedStaff;
    public TextMeshProUGUI _buttonText;
    public TextMeshProUGUI level_num;

    void Awake()
    {
        // 에디터에서 연결해두지 않았다면
        if (spawnPoint == null)
        {
            var parent = GameObject.Find("MapParent");
            if (parent != null && parent.transform.childCount > 0)
            {
                spawnPoint = parent.transform.GetChild(0);
            }
            else
            {
                Debug.LogWarning("MapParent가 없거나 자식이 없습니다.");
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
            if(spawnPoint != null)
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
