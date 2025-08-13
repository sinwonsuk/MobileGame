public struct ChangeSkinHandler : IEvent
{
    public ChangeSkinHandler(Skin skin)
    {
        this.skin = skin;
    }

    public Skin skin { get; set; }

}
