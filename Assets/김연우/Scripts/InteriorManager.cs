using System;
using System.Collections.Generic;
using UnityEngine;

public class InteriorManager : MonoBehaviour
{
    public static InteriorManager Instance { get; private set; }

    [Header("Config: 모든 인테리어 데이터")]
    public InteriorData[] allInteriors;

    [Header("Runtime: 인테리어 상태 데이터 (SO가 진실)")]
    public RunTimeInteriorData[] allRunTimeInteriors;

    public List<InteriorSlot> slots = new List<InteriorSlot>();
    public event Action OnInteriorChanged;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        DontDestroyOnLoad(gameObject);

        // 슬롯 초기화: SO 쌍을 그대로 연결
        slots.Clear();
        for (int i = 0; i < allInteriors.Length; i++)
        {
            slots.Add(new InteriorSlot(allInteriors[i], allRunTimeInteriors[i]));
            // 런타임 인스턴스는 항상 시작 시 null에서 출발
            allRunTimeInteriors[i].instance = null;
        }

        // SO의 isUsed 상태를 기준으로 설치 복원
        RefreshInstalledInteriors();
    }

    public void AcquireInterior(string name)
    {
        var slot = slots.Find(s => s.data.interiorName == name);
        if (slot == null) return;

        slot.runtimeData.isOwned = true;
        OnInteriorChanged?.Invoke();
    }


    public void UseInterior(string name)
    {
        var slot = slots.Find(s => s.data.interiorName == name);
        if (slot == null || !slot.runtimeData.isOwned) return;

        // 인스턴스가 없으면 설치, 있으면 해제
        if (slot.runtimeData.instance == null)
        {
            Vector3 pos = slot.data.placementPosition;
            var go = Instantiate(slot.data.prefab, pos, Quaternion.identity);


            slot.runtimeData.instance = go;
            slot.runtimeData.isUsed = true;
        }
        else
        {
            // 해제
            if (slot.runtimeData.instance != null)
            {
                Destroy(slot.runtimeData.instance);
            }
            slot.runtimeData.instance = null;
            slot.runtimeData.isUsed = false;
        }

        OnInteriorChanged?.Invoke();
    }

    public void RefreshInstalledInteriors()
    {
        foreach (var slot in slots)
        {
            // 중복 인스턴스 제거
            if (slot.runtimeData.instance != null)
            {
                Destroy(slot.runtimeData.instance);
                slot.runtimeData.instance = null;
            }

            if (slot.runtimeData.isOwned && slot.runtimeData.isUsed)
            {
                Vector3 pos = slot.data.placementPosition;
                var go = Instantiate(slot.data.prefab, pos, Quaternion.identity);



                slot.runtimeData.instance = go;
            }
        }

        OnInteriorChanged?.Invoke();
    }
}
