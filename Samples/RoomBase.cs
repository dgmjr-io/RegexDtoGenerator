namespace Us.Zoom;

public abstract record class RoomBase : IRoom
{
    public virtual Uri? Url { get; }
}
