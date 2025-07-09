using UnityEngine;

public class MenuBoard : MonoBehaviour
{
    private void OnEnable()
    {
        //EventBus<MenuBoardSlotSpawnHandler>.Raise(new MenuBoardSlotSpawnHandler());
    }

    private void OnDisable()
    {
        EventBus<MenuBoardSlotDeleteHandler>.Raise(new MenuBoardSlotDeleteHandler());
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [SerializeField] private Transform parentTransform;

    public Transform ParentTransform
    {
        get => parentTransform;
    }
}
