using TMPro;
using UnityEngine;

public class DamageTextManager : MonoBehaviour
{
    public static DamageTextManager Instance;

    [SerializeField] private GameObject damageTextPrefab;
    [SerializeField] private Transform canvasTransform;

    void Awake()
    {
        // 씬에 하나만 존재
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject); // 원한다면
    }

    public void ShowDamage(Vector3 worldPos, float damage, bool isCrit = false)
    {
        if (damageTextPrefab == null || canvasTransform == null) return;

        // 몬스터 중심에서 머리 위로 올리기
        Vector3 spawnPos = worldPos + Vector3.up * 1f;

        var go = Instantiate(damageTextPrefab, spawnPos, Quaternion.identity, canvasTransform);

        var dt = go.GetComponent<DamageTextUI>() ?? go.GetComponentInChildren<DamageTextUI>();
        if (dt == null) { Debug.LogError("DamageTextUI 누락!"); return; }

        string txt = Mathf.RoundToInt(damage).ToString();
        Color col = isCrit ? Color.yellow : Color.white;

        dt.Init(txt, col);
    }
}
