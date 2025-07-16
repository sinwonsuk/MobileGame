using UnityEngine;
public class uiset : MonoBehaviour
{
    [SerializeField] private GameObject target;

       public void OnButtonClicked()
    {
        bool shouldOpen = !target.activeSelf;
        target.SetActive(shouldOpen);
    }


}
