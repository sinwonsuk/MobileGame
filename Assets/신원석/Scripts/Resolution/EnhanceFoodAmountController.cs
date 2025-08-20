using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnhanceFoodAmountController : MonoBehaviour
{

    private void OnDisable()
    {

    }

    private void OnEnable()
    {
        button.interactable = true;
    }

    void Start()
    {

    }

    void Update()
    {
        FoodData data = enhanceFoodUI.foodData;

        var materials = data.enhanceSteps[data.Level].ingredients;

        var sucessRate = data.enhanceSteps[data.Level].successRate;

        int random = Random.Range(0, 100);

        for (int i = 0; i < materials.Count; i++)
        {
            if (materials[i].quantity > InventoryManager.Instance.GetItemQty(materials[i].indate))
            {
                button.interactable = false;
                return;
            }
        }
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
            enhanceResult.Image.sprite = Resources.Load<Sprite>("fail");
            enhanceResult.Text.text = "강화실패";
        }
        else // 애는 성공 
        {
            enhanceResult.Image.sprite = Resources.Load<Sprite>("success");
            enhanceResult.Text.text = "강화성공";
			data.Level += 1;
            data.isDirty = true;
           
		}
		AutoSaveManager.Instance?.ForceFlushSoon(0.25f);
	}

    [SerializeField] Button button;

    [SerializeField] EnhanceFoodUI enhanceFoodUI;
    [SerializeField] EnhanceResult enhanceResult;


    public Transform MenuParentTransform { get; set; }
}
