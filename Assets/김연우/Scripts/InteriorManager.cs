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
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        DontDestroyOnLoad(gameObject);
        // 슬롯 초기화
        for (int i = 0; i < allInteriors.Length; i++)
        {
            slots.Add(new InteriorSlot(allInteriors[i], allRunTimeInteriors[i]));
        }
        OnInteriorChanged?.Invoke();
    }

    /// <summary>인테리어 획득(구매) 처리</summary>
    public void AcquireInterior(string name)
    {
        var slot = slots.Find(s => s.data.interiorName == name);
        if (slot == null) return;
        slot.runtimeData.isOwned = true;
        OnInteriorChanged?.Invoke();
    }

    /// <summary>설치/해제 토글</summary>
    public void UseInterior(string name)
    {
        var slot = slots.Find(s => s.data.interiorName == name);
        if (slot == null || !slot.runtimeData.isOwned) return;

        if (!slot.runtimeData.isUsed)
        {
            // SO에 저장된 위치·회전으로 인스턴스 생성
            Vector3 pos = slot.data.placementPosition;
            Quaternion rot = Quaternion.Euler(slot.data.placementRotation);

            var go = Instantiate(slot.data.prefab, pos, rot);
            slot.runtimeData.instance = go;
            slot.runtimeData.isUsed = true;
        }
        else
        {
            // 기존 인스턴스 제거
            if (slot.runtimeData.instance != null)
                Destroy(slot.runtimeData.instance);
            slot.runtimeData.instance = null;
            slot.runtimeData.isUsed = false;
        }

        OnInteriorChanged?.Invoke();
    }


}
