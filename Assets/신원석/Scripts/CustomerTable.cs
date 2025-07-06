using UnityEngine;

public class CustomerTable : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //if(IsSittingAtTable==false)
    }

    public bool IsSittingAtTable { get; set; }

    [SerializeField] Transform targetTransform;

}
