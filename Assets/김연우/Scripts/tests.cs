using UnityEngine;

public class tests : MonoBehaviour
{
    void Start()
    {
        InteriorManager.Instance.LoadInteriorStates();

        // 2. 인테리어 설치/동기화 (자동 생성)
        InteriorManager.Instance.RefreshInstalledInteriors();
    }


}
