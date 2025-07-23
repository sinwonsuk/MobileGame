using UnityEngine;

[CreateAssetMenu(menuName = "Config/CameraSlideManagerConfig")]
public class CameraSlideManagerConfig : BaseScriptableObject
{
    public Transform cameraTransform;

    public CameraSlideManagerConfig()
    {
        type = typeof(CameraSlideManagerConfig);
    }
}