using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuBoardSlot : MonoBehaviour
{
    [SerializeField] Image iconImage;
    [SerializeField] TMP_Text numberText;
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text explanationText;


    public int Count { get; set; }

    public TMP_Text NumberText { get => numberText; }
    public Image IconImage { get => iconImage; }

    public void Init(Sprite icon, string number, string name, string explanation = null)
    {
        iconImage.sprite = icon;
        numberText.text = number;
        nameText.text = name;
        if (explanation != null)
            explanationText.text = explanation;

        Count = int.Parse(number);
    }
}
