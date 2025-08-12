using UnityEngine;

public class ExitDungeon : MonoBehaviour
{

    public GameObject dungeonInGameUI;
    public void OnClickMainMenu()
    {
        dungeonInGameUI.SetActive(false);

        EventBus<DungeonSlideToggleEvent>.Raise(new DungeonSlideToggleEvent(false));
        var hunters = Object.FindObjectsByType<ToggleHunterShopCanvas>(
            FindObjectsInactive.Include,      // 비활성 포함해서 찾기
            FindObjectsSortMode.None
        );
        foreach (var h in hunters)
        {
            if (h && h.gameObject.activeSelf)
                h.gameObject.SetActive(false);
        }
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
