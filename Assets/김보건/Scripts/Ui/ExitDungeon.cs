using UnityEngine;

public class ExitDungeon : MonoBehaviour
{

    public GameObject dungeonInGameUI;
    public void OnClickMainMenu()
    {
        dungeonInGameUI.SetActive(false);
        var gameController = FindAnyObjectByType<GameController>();
        if (gameController != null)
        {
            var dungeonManager = gameController.GetManager<DungeonManager>();
            if (dungeonManager != null)
                dungeonManager.ExitDungeon();
        }
    }
}
