using UnityEngine;

public interface ICooldownReadable
{
    float GetCooldownRatio(); // 0~1
}

public class SkillCooldownBar : MonoBehaviour
{
    [SerializeField] private Transform fillBarTransform;
    [SerializeField] private MonoBehaviour skillComponent;// 임의 스킬
    private ICooldownReadable skill;

    private Vector3 originalScale;

    void Awake()
    {
        if (skillComponent != null)
            skill = skillComponent as ICooldownReadable;
    }

    void Start()
    {
        if (fillBarTransform != null)
            originalScale = fillBarTransform.localScale;
    }

    void Update()
    {
        if (skill == null || fillBarTransform == null) return;

        float ratio = skill.GetCooldownRatio();  // 0~1 사이 값
        fillBarTransform.localScale = new Vector3(ratio * originalScale.x, originalScale.y, originalScale.z);
    }

    // 코드로 할당하고 싶을 때
    public void SetSkill(ICooldownReadable readable, MonoBehaviour componentRef)
    {
        skill = readable;
        skillComponent = componentRef;
    }
}
