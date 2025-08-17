using UnityEngine;

public class ExitDungeon : MonoBehaviour
{

    private GameController _gc;
    private CameraSlideManager _camSlide;

    private void Awake()
    {
        _gc = FindAnyObjectByType<GameController>();
        if (_gc != null)
            _camSlide = _gc.GetManager<CameraSlideManager>();
    }

    public void OnClickMainMenu()
    {
        if (_camSlide == null)
        {
            return;
        }

        _camSlide.MoveToRestaurant();
    }
}
