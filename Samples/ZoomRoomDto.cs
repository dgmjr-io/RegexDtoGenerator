namespace Foo;

public record class RoomBase
{
    public Uri? Url { get; set; }
}

[RegexDto(
    "https://zoom.us/j/(?<RoomNumber:int>[0-9]{10,11})(?:&pwd=(?<Password:string?>[a-zA-Z0-9]{6,8}))?",
    typeof(RoomBase)
)]
public partial record class ZoomRoomDtoClass { }

[RegexDto(
    "https://zoom.us/j/(?<RoomNumber:int>[0-9]{10,11})(?:&pwd=(?<Password:string?>[a-zA-Z0-9]{6,8}))?"
)]
public partial record struct ZoomRoomDtoStruct { }
