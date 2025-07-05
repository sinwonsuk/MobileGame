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
