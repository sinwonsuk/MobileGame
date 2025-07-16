using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class testmost : MonoBehaviour
{
    [SerializeField]private RunTimeIngredientData aasdf;
    public void OnButtonClicked()
    {
        aasdf.ingredientQty+=1;
        Debug.Log(aasdf.ingredientQty);
    }
}
