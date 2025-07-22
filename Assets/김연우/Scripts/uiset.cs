using UnityEngine;
public class uiset : MonoBehaviour
{
    [SerializeField] private GameObject target;

       public void OnButtonClicked()
    {
        bool shouldOpen = !target.activeSelf;
        EventBus<ShopUIEvent>.Raise(new ShopUIEvent(shouldOpen));
        target.SetActive(shouldOpen);
    }


}
