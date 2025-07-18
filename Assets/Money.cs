using TMPro;
using UnityEngine;

public class Money : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moneyText.text = money.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [SerializeField] TextMeshProUGUI moneyText;

    public TextMeshProUGUI MoneyText
    {
        get => moneyText;
        set => moneyText = value;
    }

    [SerializeField] int money = 10000;

}
