namespace Us.Zoom;

[RegexDto(
    @"^https://zoom.us/j/(?<RoomNumber:int>[0-9]{10,11})\?.*(?:pwd=(?<Password:string?>[a-zA-Z0-9]{6,8}))?$",
    typeof(IRoom)
)]
public partial record struct ZoomRoom
{
    public Uri Url =>
        new Uri(
            $"https://zoom.us/j/{RoomNumber}{(!IsNullOrEmpty(Password) ? $"?pwd={Password}" : "")}"
        );
}
