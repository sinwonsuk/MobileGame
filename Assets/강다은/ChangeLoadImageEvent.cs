using UnityEngine;

public struct ChangeLoadImageEvent :IEvent
{
    public ChangeLoadImageEvent(bool check)
    {
        isLoading = check;
    }



    public bool isLoading { get; set; }


}
