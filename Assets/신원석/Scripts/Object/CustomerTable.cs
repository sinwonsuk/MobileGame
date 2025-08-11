using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CustomerTable : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }


    void Update()
    {
        
    }
    private void OnEnable()
    {
        // 테이블이 활성화되면 자동 등록 이벤트 발행
        EventBus<TableAddedEvent>.Raise(new TableAddedEvent(this));
    }

    private void OnDisable()
    {
        // 비활성/파괴되면 자동 해제 이벤트 발행
        EventBus<TableRemovedEvent>.Raise(new TableRemovedEvent(this));
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {

    }

    public bool IsSittingAtTable { get; set; } = false;

    [SerializeField] Transform targetTransform;

    public Transform TargetTransform
    {
        get => targetTransform;
        set => targetTransform = value;
    }

}
