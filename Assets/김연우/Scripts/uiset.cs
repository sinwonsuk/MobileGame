using UnityEngine;
public class uiset : MonoBehaviour
{
    [SerializeField] private GameObject target;
    private bool onActive = false;

       public void OnButtonClicked()
    {
        onActive = !onActive; // ���� ���� ������
        target.SetActive(onActive);
    }


}
