using TMPro;
using UnityEngine;

public class FoodAmountController : MonoBehaviour
{

    private void OnDisable()
    {
        currentAmount = 0;
        amount.text = "0";
    }

    void Start()
    {
        amount.text = "0";
    }

    void Update()
    {
        
    }

    [SerializeField] private TextMeshProUGUI amount; 

    public void SetAmount(int amount)
    {
        this.amount.text = amount.ToString();
    }
    public void IncreaseAmount()
    {
        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click, false);

        for (int i = 0; i < foodAmountUI.IngredientPanels.Count; i++)
        {
            var amountText = foodAmountUI.IngredientPanels[i].transform.GetChild((int)IngredientPannelType.NeedIngredientAmount).GetComponent<TextMeshProUGUI>();
            var currentamountText = foodAmountUI.IngredientPanels[i].transform.GetChild((int)IngredientPannelType.CurrentIngredientAmount).GetComponent<TextMeshProUGUI>();

            int needCount = int.Parse(amountText.text);

            int CurrentCount = int.Parse(currentamountText.text);

            if(needCount >= CurrentCount)
            {
                return;
            }
        }

        for (int i = 0; i < foodAmountUI.IngredientPanels.Count; i++)
        {
            var amountText = foodAmountUI.IngredientPanels[i].transform.GetChild((int)IngredientPannelType.NeedIngredientAmount).GetComponent<TextMeshProUGUI>();

            if (int.TryParse(amountText.text, out int currentValue))
            {
                currentValue += 1;
                amountText.text = currentValue.ToString();
            }

        }

        currentAmount++;

        SetAmount(currentAmount);
    }
    public void DecreaseAmount()
    {
        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click, false);

        if (currentAmount <= 0)
            return;

        for (int i = 0; i < foodAmountUI.IngredientPanels.Count; i++)
        {
            var amountText = foodAmountUI.IngredientPanels[i].transform.GetChild((int)IngredientPannelType.NeedIngredientAmount).GetComponent<TextMeshProUGUI>();

            if (int.TryParse(amountText.text, out int currentValue))
            {
                currentValue -= 1;
                amountText.text = currentValue.ToString();
            }

        }

        currentAmount--;

        SetAmount(currentAmount);
    }

    public void FoodAmountConfirmButton()
    {
        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click, false);

        //var Name = foodAmountUI.IngredientPanels[0].transform.GetChild((int)IngredientPannelType.IngredientName).GetComponent<TextMeshProUGUI>();

        foodName = foodAmountUI.foodName;

        int tempCurrentAmount = currentAmount;

        EventBus<FoodDecreaseHandler>.Raise(new FoodDecreaseHandler(foodName, tempCurrentAmount));
        EventBus<SetManagementActiveEvent>.Raise(new SetManagementActiveEvent());
        EventBus<SetMenuParentTransformHandler>.Raise(new SetMenuParentTransformHandler(this));

        if (tempCurrentAmount <= 0)
        {
            return;
        }

        EventBus<UpMenuSpawnHandler>.Raise(new UpMenuSpawnHandler(foodAmountUI.FoodIcon, tempCurrentAmount.ToString(), foodName, MenuParentTransform));
        EventBus<MenuBoardSlotSpawnHandler>.Raise(new MenuBoardSlotSpawnHandler(tempCurrentAmount.ToString(), foodName));
    }

    public void test()
    {
        EventBus<SetManagementActiveEvent>.Raise(new SetManagementActiveEvent());
    }


    private int currentAmount;
    private string foodName;
    [SerializeField] FoodAmountUI foodAmountUI;
    public Transform MenuParentTransform { get; set; }
}
