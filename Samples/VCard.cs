using System.Data;

/*
 * VCard.cs
 *
 *   Created: 2022-12-29-10:28:20
 *   Modified: 2022-12-29-10:28:21
 *
 *   Author: David G. Mooore, Jr. <david@dgmjr.io>
 *
 *   Copyright © 2022-2023 David G. Mooore, Jr., All Rights Reserved
 *      License: MIT (https://opensource.org/licenses/MIT)
 */

namespace Contacts;

// [RegexDto(VCard.VCardRegex)]
// public partial record class VCard
// {
//    public const string AddressRegex = @"ADR;TYPE=(?<Type:AddressType>\w+):(?<StreetAddress>\w+);(?<Locality>\w+);(?<Region>\w+);(?<PostalCode:int>\w+);(?<Country>\w+)";
//    public const string BirthdayRegex = @"BDAY:(?<Birthday:DateOnly>(?<Year:int>\d{4})-(?<Month:int>\d{2})-(?<Day:int>\d{2}))";
//    public const string VersionRegex = @"VERSION:(?<Version:Version>(?<Major:int>\d+)\.(?<Minor:int>\d+))";
//    public const string VCardRegex =
//    $$""""
//    BEGIN:VCARD(?:
//    (FN=(?<FormattedName>\w+))?
//    (?:{{VersionRegex}}))?(?:
//    (?<Address>))?(?:{{AddressRegex}}))?(?:
//    (?<Birthday>{{BirthdayRegex}}))?\r\nEND:VCARD";
//    """";

//    // public virtual Version Version { get; init; }

//    public VCardPostalAddress Address { get; init; }
//    public DateOnly Birthday { get; init; }
// }

// [RegexDto(VCard.VersionRegex)]
// public partial record struct VersionDto
// {
//    // public partial int Major { get; set; }
//    // public partial int Minor { get; set; }
// }

// [RegexDto(VCard.AddressRegex, typeof(USPostalAddress))]
// public partial record class VCardPostalAddress
// {
//    public virtual AddressType Type { get; set; }
// }



public enum AddressType
{
    other,
    home,
    work,
    school
}
