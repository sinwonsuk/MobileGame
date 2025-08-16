using UnityEngine;

public class statUpdate : StatUpgradeButton
{
    void Awake()
    {
        playerStats.basicAtk = BackendGameData.Instance.userData.basicAtk;
        playerStats.RecalculateFromBasicAtk();
    }

}
