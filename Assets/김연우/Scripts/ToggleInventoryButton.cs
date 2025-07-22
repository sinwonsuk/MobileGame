using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ToggleInventoryButton : MonoBehaviour
{


    public void ToggleInventory()
    {
        EventBus<ToggleInventoryEvent>.Raise(new ToggleInventoryEvent());
    }
}
