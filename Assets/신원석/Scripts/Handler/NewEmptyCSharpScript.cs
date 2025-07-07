using UnityEngine;
using UnityEngine.UI;

public struct MenuSpawnHandler : IEvent
{
    // Constructor must have a return type. Fixed by adding 'public' and specifying the struct name as the return type.
    public MenuSpawnHandler(Image image,string number,string name)
    {
        Image = image;
        this.number = number;
        Name = name;
    }


    public Image Image { get; set; }
    public string number { get; set; }

    public string Name { get; set; }
}
