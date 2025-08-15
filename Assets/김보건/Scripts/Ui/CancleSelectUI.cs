using UnityEngine;

public class CancleSelectUI : MonoBehaviour
{
    public GameObject selectUI;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void OnClickCancle()
    {
        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click, false);
        selectUI.SetActive(false);
        ButtonManager.buttonClick = ButtonClick.none;
    }
}
