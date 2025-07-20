using UnityEngine;

public class DropSpread : MonoBehaviour
{
    [SerializeField] private float forceMin = 4f;
    [SerializeField] private float forceMax = 6f;
    [SerializeField] private float eatEnableDelay = 0.5f;

    private EatItem eatItem;

    void Start()
    {
        // 초기화 시점에 폭발 방향 부여
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 randomDir = Random.insideUnitCircle.normalized;
            float force = Random.Range(forceMin, forceMax);
            rb.AddForce(randomDir * force, ForceMode2D.Impulse);
        }

        // EatItem 비활성화 후 0.5초 뒤에 다시 켜기
        eatItem = GetComponent<EatItem>();
        if (eatItem != null)
        {
            eatItem.enabled = false;
            StartCoroutine(EnableEatAfterDelay());
        }
    }

    private System.Collections.IEnumerator EnableEatAfterDelay()
    {
        yield return new WaitForSeconds(eatEnableDelay);
        if (eatItem != null)
            eatItem.enabled = true;
    }
}
