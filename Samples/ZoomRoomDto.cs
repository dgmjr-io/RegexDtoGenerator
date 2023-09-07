namespace Foo;

public abstract record class RoomBase
{
    public virtual Uri? Url { get; }
}

[RegexDto(
    @"^https://zoom.us/j/(?<RoomNumber:int>[0-9]{10,11})\?.*(?:pwd=(?<Password:string?>[a-zA-Z0-9]{6,8}))?$",
    typeof(RoomBase)
)]
public partial record class ZoomRoomDtoClass
{
    public override Uri Url => new Uri($"https://zoom.us/j/{RoomNumber}{(!string.IsNullOrEmpty(Password) ? $"?pwd={Password}" : "")}");
}

[RegexDto(
    @"^https://zoom.us/j/(?<RoomNumber:int>[0-9]{10,11})\?.*(?:pwd=(?<Password:string?>[a-zA-Z0-9]{6,8}))?$"
)]
public partial record struct ZoomRoom
{
    public Uri Url => new Uri($"https://zoom.us/j/{RoomNumber}{(!string.IsNullOrEmpty(Password) ? $"?pwd={Password}" : "")}");
}
