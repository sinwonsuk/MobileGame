using TMPro;
using UnityEngine;

public class Reputation : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        reputationText.text = BackendGameData.Instance.userData.reputation.ToString();
    }

    [SerializeField] TextMeshProUGUI reputationText;
}
