using UnityEngine;

public class GoButtonController : MonoBehaviour
{
    private void Start()
    {
        EventBus<ButtonisActiveHandler>.OnEvent += OnGlobalUIActiveChanged;
    }

    //private void OnDestroy()
    //{
    //    EventBus<ButtonisActiveHandler>.OnEvent -= OnGlobalUIActiveChanged;
    //}

    private void OnGlobalUIActiveChanged(ButtonisActiveHandler e)
    {
        gameObject.SetActive(e.isActive);
    }
}