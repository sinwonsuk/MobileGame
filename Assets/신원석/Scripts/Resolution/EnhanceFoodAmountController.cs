using TMPro;
using UnityEngine;

public class EnhanceFoodAmountController : MonoBehaviour
{

    private void OnDisable()
    {

    }

    void Start()
    {

    }

    void Update()
    {

    }


    public void FoodAmountConfirmButton()
    {

        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click,false);


        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Enhance, false);

        FoodData data = enhanceFoodUI.foodData;

        var materials = data.enhanceSteps[data.Level].ingredients;

        var sucessRate = data.enhanceSteps[data.Level].successRate;

        int random = Random.Range(0, 100);

        for (int i = 0; i < materials.Count; i++)
        {
            if (materials[i].quantity > InventoryManager.Instance.GetItemQty(materials[i].indate))
            {
                return;
            }
        }


        for (int i = 0; i < materials.Count; i++)
        {
            EventBus<EnhanceFoodDecreaseHandler>.Raise(new EnhanceFoodDecreaseHandler(data.displayName, materials[i].indate, materials[i].quantity));
        }

        enhanceResult.gameObject.SetActive(true);

        if (random > sucessRate) // 애는 실패
        {
            enhanceResult.Text.text = "강화실패";
        }
        else // 애는 성공 
        {
            enhanceResult.Text.text = "강화성공";
			data.Level += 1;
            data.isDirty = true;
		}






        //EventBus<UpMenuSpawnHandler>.Raise(new UpMenuSpawnHandler(foodAmountUI.FoodIcon, tempCurrentAmount.ToString(), foodName, MenuParentTransform));
        //EventBus<MenuBoardSlotSpawnHandler>.Raise(new MenuBoardSlotSpawnHandler(tempCurrentAmount.ToString(), foodName));
    }


    [SerializeField] EnhanceFoodUI enhanceFoodUI;
    [SerializeField] EnhanceResult enhanceResult;


    public Transform MenuParentTransform { get; set; }
}
