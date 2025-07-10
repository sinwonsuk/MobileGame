using UnityEngine;
using UnityEngine.UI;

public struct UpMenuSpawnHandler : IEvent
{
    // Constructor must have a return type. Fixed by adding 'public' and specifying the struct name as the return type.
    public UpMenuSpawnHandler(Image image,string number,string name, Transform transform)
    {
        Image = image;
        this.number = number;
        Name = name;
        parentTransform = transform;    
    }
    public Transform parentTransform { get; set; }

    public Image Image { get; set; }
    public string number { get; set; }

    public string Name { get; set; }
}

public struct MenuBoardSlotSpawnHandler : IEvent
{
    //public MenuBoardSlotSpawnHandler(Transform transform)
    //{
    //    parentTransform = transform;
    //}

    //public Transform parentTransform { get; set; }
}

public struct MenuBoardSlotDeleteHandler : IEvent
{

}
