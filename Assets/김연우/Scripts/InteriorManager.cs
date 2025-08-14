using System;
using System.Collections.Generic;
using UnityEngine;

public class InteriorManager : MonoBehaviour
{
    public static InteriorManager Instance { get; private set; }

    [Header("Config: 모든 인테리어 데이터")]
    public InteriorData[] allInteriors;
    [Header("Runtime: 인테리어 상태 데이터")]
    public RunTimeInteriorData[] allRunTimeInteriors;

    public List<InteriorSlot> slots = new List<InteriorSlot>();
    public event Action OnInteriorChanged;

    private void Awake()
    {
         /*PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();*/
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        DontDestroyOnLoad(gameObject);

        // 슬롯 초기화
        for (int i = 0; i < allInteriors.Length; i++)
        {
            slots.Add(new InteriorSlot(allInteriors[i], allRunTimeInteriors[i]));
        }

        LoadInteriorStates();
    }

    // 상태 저장
    public void SaveInteriorStates()
    {
        foreach (var slot in slots)
        {
            string key = slot.data.interiorName;
            PlayerPrefs.SetInt(key + "_isOwned", slot.runtimeData.isOwned ? 1 : 0);
            PlayerPrefs.SetInt(key + "_isUsed", slot.runtimeData.isUsed ? 1 : 0);
        }
        PlayerPrefs.Save();
    }

    // 상태 불러오기
    public void LoadInteriorStates()
    {
        foreach (var slot in slots)
        {
            string key = slot.data.interiorName;
            slot.runtimeData.isOwned = PlayerPrefs.GetInt(key + "_isOwned", slot.runtimeData.isOwned ? 1 : 0) == 1;
            slot.runtimeData.isUsed = PlayerPrefs.GetInt(key + "_isUsed", slot.runtimeData.isUsed ? 1 : 0) == 1;
        }
    }

    // 인테리어 획득(구매)
    public void AcquireInterior(string name)
    {
        var slot = slots.Find(s => s.data.interiorName == name);

        if (slot == null) return;
        slot.runtimeData.isOwned = true;
        SaveInteriorStates();
        OnInteriorChanged?.Invoke();
    }

    // 설치/해제 토글 (instance==null 기준!)
    public void UseInterior(string name)
    {
        var slot = slots.Find(s => s.data.interiorName == name);
        if (slot == null || !slot.runtimeData.isOwned) return;

        // 인스턴스가 없으면 설치, 있으면 해제
        if (slot.runtimeData.instance == null)
        {
            Vector3 pos = slot.data.placementPosition;
            var go = Instantiate(slot.data.prefab, pos, Quaternion.identity);

            // PiggyBank 등 SO 연결 및 누적금 복원!
            var piggyBank = go.GetComponent<PiggyBank>();
            if (piggyBank != null)
            {
                piggyBank.runtimeData = slot.runtimeData;
                piggyBank.RestoreAccumulated();
            }

            slot.runtimeData.instance = go;
            slot.runtimeData.isUsed = true;
        }
        else
        {
            if (slot.runtimeData.instance != null)
            {
                // PiggyBank면 누적금도 리셋
                var piggyBank = slot.runtimeData.instance.GetComponent<PiggyBank>();
                if (piggyBank != null)
                    piggyBank.ResetPiggyBank();

                Destroy(slot.runtimeData.instance);
            }
            slot.runtimeData.instance = null;
            slot.runtimeData.isUsed = false;
        }

        SaveInteriorStates();
        OnInteriorChanged?.Invoke();
    }

    /// <summary>
    /// 로그인/서버 동기화 후 호출! isUsed==true인 인테리어 자동 설치
    /// </summary>
    public void RefreshInstalledInteriors()
    {
        foreach (var slot in slots)
        {
            // 기존 인스턴스 있으면 제거 (중복 방지)
            if (slot.runtimeData.instance != null)
            {
                Destroy(slot.runtimeData.instance);
                slot.runtimeData.instance = null;
            }

            // isUsed==true면 설치
            if (slot.runtimeData.isUsed)
            {
                Vector3 pos = slot.data.placementPosition;
                var go = Instantiate(slot.data.prefab, pos, Quaternion.identity);
                var piggyBank = go.GetComponent<PiggyBank>();
                if (piggyBank != null)
                {
                    piggyBank.runtimeData = slot.runtimeData;
                    piggyBank.RestoreAccumulated();
                }
                slot.runtimeData.instance = go;
            }
        }
        OnInteriorChanged?.Invoke();
    }
}
