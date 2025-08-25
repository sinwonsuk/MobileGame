using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DungeonLightFlasher : MonoBehaviour
{
    [SerializeField] private Light dirLight;
    [SerializeField] private float baseIntensity = 1f;

    void Awake()
    {
        if (!dirLight) dirLight = GetComponent<Light>();
        if (dirLight) baseIntensity = dirLight.intensity;
    }

    public void Flash(float dim = 0.2f, float fadeIn = 0.06f, float hold = 0.05f, float fadeOut = 0.18f)
    {
        if (!dirLight) return;
        StopAllCoroutines();
        StartCoroutine(CoFlash(dim, fadeIn, hold, fadeOut));
    }

    private IEnumerator CoFlash(float dim, float fadeIn, float hold, float fadeOut)
    {
        float start = dirLight.intensity;

        // ´Ù¿î
        for (float t = 0; t < fadeIn; t += Time.unscaledDeltaTime)
        {
            dirLight.intensity = Mathf.Lerp(start, dim, t / fadeIn);
            yield return null;
        }
        dirLight.intensity = dim;

        yield return new WaitForSecondsRealtime(hold);

        // º¹±Í
        for (float t = 0; t < fadeOut; t += Time.unscaledDeltaTime)
        {
            dirLight.intensity = Mathf.Lerp(dim, baseIntensity, t / fadeOut);
            yield return null;
        }
        dirLight.intensity = baseIntensity;
    }
}
