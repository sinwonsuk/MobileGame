using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum IngredientPannelType
{
    IngredientImage,
    NeedIngredientAmount,
    CurrentIngredientAmount,
    IngredientName,
}

public class FoodAmountUI : MonoBehaviour
{
    private void OnEnable()
    {
        EventBus<IngredientsPannelSpawnHandler>.OnEvent += CreateIngredientAmountSlot;
        EventBus<FoodSlotHandler>.OnEvent += ChangeImage;
    }
    private void OnDisable()
    {
        EventBus<IngredientsPannelSpawnHandler>.OnEvent -= CreateIngredientAmountSlot;
        EventBus<FoodSlotHandler>.OnEvent -= ChangeImage;


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
        GameObject obg =Instantiate(IngredientAmountPanel, transformfoodAmountPanel);

        // Set the food icon
        Sprite foodSprite = Resources.Load<Sprite>(ingredientsPannelSpawnHandler.Image);
        obg.transform.GetChild((int)IngredientPannelType.IngredientImage).GetComponent<Image>().sprite = foodSprite;

        // Set the current ingredient amount to 0
        obg.transform.GetChild((int)IngredientPannelType.NeedIngredientAmount).GetComponent<TextMeshProUGUI>().text = ingredientsPannelSpawnHandler.NeedIngredientAmount.ToString();

        // Set the need ingredient amount
        obg.transform.GetChild((int)IngredientPannelType.CurrentIngredientAmount).GetComponent<TextMeshProUGUI>().text = ingredientsPannelSpawnHandler.CurrentIngredientAmount.ToString();

        obg.transform.GetChild((int)IngredientPannelType.IngredientName).GetComponent<TextMeshProUGUI>().text = ingredientsPannelSpawnHandler.IngredientName.ToString();

        IngredientPanels.Add(obg);
    }

    public void ChangeImage(FoodSlotHandler foodSlotHandler)
    {
        Sprite foodSprite = Resources.Load<Sprite>(foodSlotHandler.Image);
        foodIcon.sprite = foodSprite;
    }

    [SerializeField] Image foodIcon;
    [SerializeField] GameObject IngredientAmountPanel;
    [SerializeField] Transform transformfoodAmountPanel;

    public Image FoodIcon => foodIcon;


    public List<GameObject> IngredientPanels { get; set; } = new List<GameObject>();
}
