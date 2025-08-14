using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//public enum IngredientPannelType
//{
//    IngredientImage,
//    NeedIngredientAmount,
//    CurrentIngredientAmount,
//    IngredientName,
//}

public class EnhanceFoodUI : MonoBehaviour
{
    private void OnEnable()
    {
        EventBus<IngredientsPannelSpawnHandler>.OnEvent += CreateIngredientAmountSlot;
        EventBus<EnhanceFoodSlotHandler>.OnEvent += ChangeImage;
    }
    private void OnDisable()
    {
        EventBus<IngredientsPannelSpawnHandler>.OnEvent -= CreateIngredientAmountSlot;
        EventBus<EnhanceFoodSlotHandler>.OnEvent -= ChangeImage;


        for (int i = 0; i < IngredientPanels.Count; i++)
        {
            Destroy(IngredientPanels[i]);
        }

        IngredientPanels.Clear();
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CreateIngredientAmountSlot(IngredientsPannelSpawnHandler ingredientsPannelSpawnHandler)
    {
        GameObject obg = Instantiate(IngredientAmountPanel, transformfoodAmountPanel);

        // Set the food icon
        Sprite foodSprite = Resources.Load<Sprite>(ingredientsPannelSpawnHandler.Image);
        obg.transform.GetChild((int)IngredientPannelType.IngredientImage).GetComponent<Image>().sprite = foodSprite;

        // Set the current ingredient amount to 0
        obg.transform.GetChild((int)IngredientPannelType.CurrentIngredientAmount).GetComponent<TextMeshProUGUI>().text = ingredientsPannelSpawnHandler.NeedIngredientAmount.ToString();

        // Set the need ingredient amount
        obg.transform.GetChild((int)IngredientPannelType.NeedIngredientAmount).GetComponent<TextMeshProUGUI>().text = ingredientsPannelSpawnHandler.CurrentIngredientAmount.ToString();

        obg.transform.GetChild((int)IngredientPannelType.IngredientName).GetComponent<TextMeshProUGUI>().text = ingredientsPannelSpawnHandler.IngredientName.ToString();

        IngredientPanels.Add(obg);
    }

    public void ChangeImage(EnhanceFoodSlotHandler foodSlotHandler)
    {
        Sprite foodSprite = Resources.Load<Sprite>(foodSlotHandler.Image);

        foodIcon.sprite = foodSprite;
        foodName = foodSlotHandler.name;

        currentLevel.text = foodSlotHandler.CurrentLevel.ToString();
        currentMoney.text = foodSlotHandler.CurrentPrice.ToString();
        futureLevel.text = foodSlotHandler.FutureLevel.ToString();
        futureMoney.text = foodSlotHandler.FuturePrice.ToString();

        MaxLevel = foodSlotHandler.maxLevel;

        foodData = foodSlotHandler.foodData;
    }

    [SerializeField] Image foodIcon;

    [SerializeField] TextMeshProUGUI currentLevel;
    [SerializeField] TextMeshProUGUI currentMoney;
    [SerializeField] TextMeshProUGUI futureLevel;
    [SerializeField] TextMeshProUGUI futureMoney;

    public TextMeshProUGUI FutureLevel
    {
        get => futureLevel;
        set => futureLevel = value;
    }

    public TextMeshProUGUI FutureMoney
    {
        get => futureMoney;
        set => futureMoney = value;
    }
    public TextMeshProUGUI CurrentLevel
    {
        get => currentLevel;
        set => currentLevel = value;
    }
    [SerializeField] GameObject IngredientAmountPanel;
    [SerializeField] Transform transformfoodAmountPanel;

    public Image FoodIcon => foodIcon;

    public string foodName { get; set; }

    public int MaxLevel;

    public FoodData foodData { get; set; }

    public List<GameObject> IngredientPanels { get; set; } = new List<GameObject>();
}
