namespace Us.Zoom;

public abstract record class RoomBase : IRoom
{
    [@StringSyntax(StringSyntax.Uri)]
    public virtual Uri? Url { get; }
}
