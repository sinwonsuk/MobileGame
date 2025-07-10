public struct MenuBoardActiveHandler : IEvent
{
    public MenuBoardActiveHandler(bool isActive)
    {
        this.isActive = isActive;

    }

    public bool isActive { get; set; }


}