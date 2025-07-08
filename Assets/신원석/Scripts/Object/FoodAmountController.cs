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

    // Update is called once per frame
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
        var Name = foodAmountUI.IngredientPanels[0].transform.GetChild((int)IngredientPannelType.IngredientName).GetComponent<TextMeshProUGUI>();

        foodName = Name.text;

        int tempCurrentAmount = currentAmount;

        EventBus<FoodDecreaseHandler>.Raise(new FoodDecreaseHandler(foodName, currentAmount));
        EventBus<SetManagementActiveEvent>.Raise(new SetManagementActiveEvent());
        EventBus<MenuSpawnHandler>.Raise(new MenuSpawnHandler(foodAmountUI.FoodIcon, tempCurrentAmount.ToString(), foodName));

    }

    private int currentAmount;
    private string foodName;
    [SerializeField] FoodAmountUI foodAmountUI;

}
