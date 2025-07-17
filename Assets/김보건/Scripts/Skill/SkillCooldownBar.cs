using UnityEngine;

public class SkillCooldownBar : MonoBehaviour
{
    [SerializeField] private Transform fillBarTransform;  
    public BigBulletSkill skill;        

    private Vector3 originalScale;

    void Start()
    {
        if (fillBarTransform != null)
            originalScale = fillBarTransform.localScale;
    }

    void Update()
    {
        if (skill == null || fillBarTransform == null) return;

        float ratio = skill.GetCooldownRatio();  // 0~1 »çÀÌ °ª
        fillBarTransform.localScale = new Vector3(ratio * originalScale.x, originalScale.y, originalScale.z);
    }
}
