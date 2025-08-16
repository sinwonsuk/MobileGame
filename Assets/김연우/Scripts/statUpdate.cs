using UnityEngine;

public class statUpdate : StatUpgradeButton
{
    void Awake()
    {
        BackendGameData.Instance.userData.basicAtk = 1f;
        playerStats.basicAtk = BackendGameData.Instance.userData.basicAtk;
        playerStats.RecalculateFromBasicAtk();
    }

}
