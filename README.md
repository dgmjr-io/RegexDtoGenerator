---
author: David G. Mooore, Jr.
author_email: david@dgmjr.io
title: Regex DTO Generator README
modified: 2022-12-28-07:31:51
created: 2022-12-28-07:31:50
license: MIT
---

# Regex DTO Generator

This is a simple tool to generate DTOs from a regex. It is a work in progress.

## Usage

Simply decorate any `class`, `record`, `struct`, `record class`, or
`record struct` with the attribute `[RegexDtoAttribute]` and provide a regular expression with names groups.  You can extend the regular expression syntax by including the primitive data types in the name group separated from the name by a colon (`:`).  The following data types are supported:

    - `string`
    - `int`
    - `long`
    - `float`
    - `double`
    - `decimal`
    - `bool`
    - `DateTime`
    - `DateTimeOffset`
    - `TimeSpan`
    - `Guid`
    - `Uri`
    - Really, any type that implements `IConvertible` and is convertible to/from a `string`

Let's say you want to pull out the room number and password from a Zoom URL.  You can do this with the following code:

```csharp title="ZoomRoom.cs"

[RegexDto("https://zoom.us/j/(?<RoomNumber:int>[0-9]{10,11})(?:&pwd=(?<Password:string?>[a-zA-Z0-9]{6,8}))?")]
public partial record struct ZoomRoom { }
```

This will generate the following code for you:

```csharp title="ZoomRoom.g.cs"

public partial record struct ZoomRoom
{
    public const string RegexString = @"""https://zoom.us/j/(?<RoomNumber>[0-9]{10,11})(?:&pwd=(?<Password>[a-zA-Z0-9]{6,8}))?""";
    public static readonly System.Text.RegularExpressions.Regex Regex = new System.Text.RegularExpressions.Regex(RegexString);

    public static ZoomRoom Parse(string s)
    {
        var match = Regex.Match(s);
        if (!match.Success)
        {
            throw new System.ArgumentException($"The string \"{s}\" does not match the regular expression \"{RegexString}\".", nameof(s));
        }

        return new ZoomRoom
        {
            RoomNumber = (int)System.Convert.ChangeType(match.Groups["RoomNumber"]?.Value, typeof(int)),
            Password = match.Groups["Password"]?.Value is null ? null : (string?)System.Convert.ChangeType(match.Groups["Password"]?.Value, typeof(string)),
        };
    }

    public int RoomNumber { get; set; }
    public string? Password { get; set; }
}

```
