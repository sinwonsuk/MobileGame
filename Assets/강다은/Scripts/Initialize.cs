using UnityEngine;

public class Initialize : MonoBehaviour
{
    void Start()
    {
        BackendGameData.Instance.Initialized();
    }
}
