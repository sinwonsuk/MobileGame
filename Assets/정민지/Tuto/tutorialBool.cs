using UnityEngine;

public class tutorialBool : MonoBehaviour
{
    public static tutorialBool Instance { get; private set; }

    [Header("튜토리얼 완료 여부 bool값")]
    public bool clearStartTuto = false; //시작 튜토
    public bool clearInvenTuto = false; //인벤 튜토
    public bool clearBuyStaffTuto = false; //경영직원 구매 튜토
    public bool clearBuyHunterTuto = false; //헌터 구매 튜토
    public bool clearShopTuto = false; //상점 튜토
    public bool clearDispatchTuto = false; //강화 및 파견지 튜토
    public bool clearLevelUpTuto = false; //요리 강화 튜토

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
