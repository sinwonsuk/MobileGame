using UnityEngine;

public class FitBackgroundToCamera : MonoBehaviour
{
    [SerializeField] private Camera cam;

    void Start()
    {
        FitToCamera();
    }

    void FitToCamera()
    {
        float height = cam.orthographicSize * 2f;
        float width = height * Screen.width / Screen.height;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Vector2 spriteSize = sr.bounds.size;

        transform.localScale = new Vector3(
            width / spriteSize.x,
            height / spriteSize.y,
            1f
        );

        transform.position = new Vector3(
            cam.transform.position.x,
            cam.transform.position.y,
            transform.position.z
        );
    }
}