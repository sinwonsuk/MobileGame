//using UnityEngine;
//using System.Collections.Generic;

///// <summary>
///// Skrypt odpowiada za usatwienie rozdzielczosci kemerze
///// </summary>
//public class CameraResolution : MonoBehaviour
//{


//    #region Pola
//    private int ScreenSizeX = 0;
//    private int ScreenSizeY = 0;
//    #endregion

//    #region metody

//    #region rescale camera
//    private void RescaleCamera()
//    {

//        if (Screen.width == ScreenSizeX && Screen.height == ScreenSizeY) return;

//        float targetaspect = 16.0f / 9.0f;
//        float windowaspect = (float)Screen.width / (float)Screen.height;
//        float scaleheight = windowaspect / targetaspect;
//        Camera camera = GetComponent<Camera>();

//        if (scaleheight < 1.0f)
//        {
//            Rect rect = camera.rect;

//            rect.width = 1.0f;
//            rect.height = scaleheight;
//            rect.x = 0;
//            rect.y = (1.0f - scaleheight) / 2.0f;

//            camera.rect = rect;
//        }
//        else // add pillarbox
//        {
//            float scalewidth = 1.0f / scaleheight;

//            Rect rect = camera.rect;

//            rect.width = scalewidth;
//            rect.height = 1.0f;
//            rect.x = (1.0f - scalewidth) / 2.0f;
//            rect.y = 0;

//            camera.rect = rect;
//        }

//        ScreenSizeX = Screen.width;
//        ScreenSizeY = Screen.height;
//    }
//    #endregion

//    #endregion

//    #region metody unity

//    void OnPreCull()
//    {
//        if (Application.isEditor) return;
//        Rect wp = Camera.main.rect;
//        Rect nr = new Rect(0, 0, 1, 1);

//        Camera.main.rect = nr;
//        GL.Clear(true, true, Color.black);

//        Camera.main.rect = wp;

//    }

//    // Use this for initialization
//    void Start()
//    {
//        RescaleCamera();
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        RescaleCamera();
//    }
//    #endregion
//}
//using UnityEngine;
//using System.Collections.Generic;
//using Unity.VisualScripting;

/// <summary>
/// Skrypt odpowiada za usatwienie rozdzielczosci kemerze
/// </summary>
//public class CameraResolution : MonoBehaviour
//{
//    void Awake()
//    {
//        Camera camera = GetComponent<Camera>();

//        Rect rect = camera.rect;

//        float scaleHeight = ((float)Screen.width / (float)Screen.height) / (9.0f / 20.0f);

//        float scaleWidth = 1.0f / scaleHeight;

//        if(scaleHeight < 1.0f)
//        {
//            rect.width = scaleHeight;
//            rect.y = (1.0f - scaleHeight) / 2.0f;
//        }
//        else
//        {
//            rect.width = scaleWidth;
//            rect.x = (1.0f - scaleWidth) / 2.0f;
//        }

//        camera.rect = rect; 
//    }

//    private void Update()
//    {
//        Camera camera = GetComponent<Camera>();

//        Rect rect = camera.rect;

//        float scaleHeight = ((float)Screen.width / (float)Screen.height) / (9.0f / 20.0f);

//        float scaleWidth = 1.0f / scaleHeight;

//        if (scaleHeight < 1.0f)
//        {
//            rect.width = scaleHeight;
//            rect.y = (1.0f - scaleHeight) / 2.0f;
//        }
//        else
//        {
//            rect.width = scaleWidth;
//            rect.x = (1.0f - scaleWidth) / 2.0f;
//        }

//        camera.rect = rect;
//    }
//}
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Camera))]
public class AspectRatio : MonoBehaviour
{
    public float targetAspectRatio = 9f / 20f; // The desired aspect ratio, e.g., 16:9
    private Camera _camera;


    void Start()
    {
        _camera = GetComponent<Camera>();

    }

    void SetCameraAspect()
    {
        float windowAspect = (float)Screen.width / Screen.height;
        float scaleHeight = windowAspect / targetAspectRatio;

        if (scaleHeight < 1.0f)
        {
            // Letterboxing
            Rect rect = _camera.rect;

            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f;

            _camera.rect = rect;
        }
        else
        {
            // Pillarboxing
            float scaleWidth = 1.0f / scaleHeight;

            Rect rect = _camera.rect;

            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f;
            rect.y = 0;

            _camera.rect = rect;
        }
    }

    private void Update()
    {
        SetCameraAspect();

    }
}