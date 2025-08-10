using UnityEngine;

// 이게 나을려나 팀들이 다 퍼트려놔서 그냥 중간다리 컨포넌트를 둘까 아니면 합칠까.. 

public class UIButtonLink : MonoBehaviour
{
    [SerializeField] private ButtonManager manager;
    [SerializeField] private ButtonClick type; // 인스펙터에서 enum로 선택

    public void Invoke() => manager.OnClick(type); // UI Button에 이걸 등록
}