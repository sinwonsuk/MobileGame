using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/RunTime Interior Data")]
public class RunTimeInteriorData : ScriptableObject
{
    public string indate;
    public string interiorName;    // 내부 식별용
    public bool isOwned;           // 구매 또는 획득 여부
    public bool isUsed;            // 현재 설치된 상태인가?
    [HideInInspector] public GameObject instance; // 설치된 인스턴스 참조

	[NonSerialized]
	public bool isDirty = false;
}
