using UnityEngine;
public class uiset : MonoBehaviour
{
    [SerializeField] private GameObject target;
    private bool onActive = false;

       public void OnButtonClicked()
    {
        onActive = !onActive; // 먼저 상태 뒤집기
        target.SetActive(onActive);
    }


}
