using UnityEngine;

public class ExitDungeon : MonoBehaviour
{

    public GameObject dungeonInGameUI;
    public void OnClickMainMenu()
    {
        dungeonInGameUI.SetActive(false);

        EventBus<DungeonSlideToggleEvent>.Raise(new DungeonSlideToggleEvent(false));

        var gameController = FindAnyObjectByType<GameController>();
        if (gameController != null)
        {
            var dungeonManager = gameController.GetManager<DungeonManager>();
            if (dungeonManager != null)
                dungeonManager.ExitDungeon();
        }


        EventBus<ButtonisActiveHandler>.Raise(new ButtonisActiveHandler(true));
    }
}
