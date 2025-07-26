using System.Collections;
using UnityEngine;

public class BuffStaff : StaffBase
{
    [SerializeField] Animator animator;
    [SerializeField] float buffRange = 5f;
    [SerializeField] LayerMask allyLayer;

    public override void Init(StaffStatsSO stats, RuntimeStaffStatsSO Runtimestats)
    {
        base.Init(stats, Runtimestats);
        StartCoroutine(ApplyBuff());
    }

    private IEnumerator ApplyBuff()
    {
        while (true)
        {
            if (animator != null)
                animator.SetTrigger("BuffTrigger");

            // 아군 찾기
            var allies = Physics2D.OverlapCircleAll(transform.position, buffRange, allyLayer);
            foreach (var ally in allies)
            {
                //버프
            }

            yield return new WaitForSeconds(3f); // 버프 간격
        }
    }
}
