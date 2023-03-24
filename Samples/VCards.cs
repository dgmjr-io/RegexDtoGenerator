/*
 * VCard.cs
 *
 *   Created: 2023-01-05
 */
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text;
using System.Globalization;
using System.Diagnostics.CodeAnalysis;

#nullable enable
#nullable enable
namespace Contacts
{
    public partial record class VCardBase
    {
        #if NET7_0_OR_GREATER
        [System.Diagnostics.CodeAnalysis.StringSyntax("regex")]
        #endif
        public const string RegexString = @"""BEGIN:VCARD(?:\r\n(FN=(?<FormattedName>\w+))?(?:\r\n(?<Version>VERSION(?<VersionVersion>(?<Majorint>\d+)\.(?<Minor>\d+))))?(?:
(?<Address>ADR;TYPE=(?<TypeAddressType>\w+):(?<StreetAddress>\w+);(?<Locality>\w+);(?<Region>\w+);(?<PostalCode>\w+);(?<Country>\w+)))?(?:
(?<Birthday>BDAY(?<BirthdayDateOnly>(?<Yearint>\d{4})-(?<Month>\d{2})-(?<Day>\d{2})))))?
END:VCARD""";

        #if NET7_0_OR_GREATER
        [GeneratedRegex(RegexString, RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline | RegexOptions.RightToLeft | RegexOptions.Singleline)]
        public static partial System.Text.RegularExpressions.Regex Regex();
        #else
        private static readonly System.Text.RegularExpressions.Regex _regex = new System.Text.RegularExpressions.Regex(RegexString);
        public static System.Text.RegularExpressions.Regex Regex() => _regex;
        #endif

        public static VCard Parse(string s)
        {
            var match = Regex().Match(s);
            if (!match.Success)
            {
                throw new System.ArgumentException($"The string \"{s}\" does not match the regular expression \"{RegexString}\".", nameof(s));
            }

            return new VCard
            {
                FormattedName = (string)System.Convert.ChangeType(match.Groups["FormattedName"]?.Value, typeof(string)),
                Version = (string)System.Convert.ChangeType(match.Groups["Version"]?.Value, typeof(string)),
                Minor = (int)System.Convert.ChangeType(match.Groups["Minor"]?.Value, typeof(int)),
                Address = (string)System.Convert.ChangeType(match.Groups["Address"]?.Value, typeof(string)),
                StreetAddress = (string)System.Convert.ChangeType(match.Groups["StreetAddress"]?.Value, typeof(string)),
                Locality = (string)System.Convert.ChangeType(match.Groups["Locality"]?.Value, typeof(string)),
                Region = (string)System.Convert.ChangeType(match.Groups["Region"]?.Value, typeof(string)),
                PostalCode = (int)System.Convert.ChangeType(match.Groups["PostalCode"]?.Value, typeof(int)),
                Country = (string)System.Convert.ChangeType(match.Groups["Country"]?.Value, typeof(string)),
                Birthday = (string)System.Convert.ChangeType(match.Groups["Birthday"]?.Value, typeof(string)),
                Month = (int)System.Convert.ChangeType(match.Groups["Month"]?.Value, typeof(int)),
                Day = (int)System.Convert.ChangeType(match.Groups["Day"]?.Value, typeof(int)),
            };
        }
        public virtual string FormattedName { get; set; }
        public virtual string Version { get; set; }
        public virtual int Minor { get; set; }
        public virtual string Address { get; set; }
        public virtual string StreetAddress { get; set; }
        public virtual string Locality { get; set; }
        public virtual string Region { get; set; }
        public virtual int PostalCode { get; set; }
        public virtual string Country { get; set; }
        public virtual string Birthday { get; set; }
        public virtual int Month { get; set; }
        public virtual int Day { get; set; }

        protected VCardBase () { }

        protected VCardBase (string s)
        {
            var match = Regex().Match(s);
            if (!match.Success)
            {
                throw new System.ArgumentException($"The string \"{s}\" does not match the regular expression \"{RegexString}\".", nameof(s));
            }

            FormattedName = (string)System.Convert.ChangeType(match.Groups["FormattedName"]?.Value, typeof(string));
            Version = (string)System.Convert.ChangeType(match.Groups["Version"]?.Value, typeof(string));
            Minor = (int)System.Convert.ChangeType(match.Groups["Minor"]?.Value, typeof(int));
            Address = (string)System.Convert.ChangeType(match.Groups["Address"]?.Value, typeof(string));
            StreetAddress = (string)System.Convert.ChangeType(match.Groups["StreetAddress"]?.Value, typeof(string));
            Locality = (string)System.Convert.ChangeType(match.Groups["Locality"]?.Value, typeof(string));
            Region = (string)System.Convert.ChangeType(match.Groups["Region"]?.Value, typeof(string));
            PostalCode = (int)System.Convert.ChangeType(match.Groups["PostalCode"]?.Value, typeof(int));
            Country = (string)System.Convert.ChangeType(match.Groups["Country"]?.Value, typeof(string));
            Birthday = (string)System.Convert.ChangeType(match.Groups["Birthday"]?.Value, typeof(string));
            Month = (int)System.Convert.ChangeType(match.Groups["Month"]?.Value, typeof(int));
            Day = (int)System.Convert.ChangeType(match.Groups["Day"]?.Value, typeof(int));
        }
    }
}

#nullable enable
namespace Contacts
{
    public partial record class VCard  : VCardBase
    {
        #if NET7_0_OR_GREATER
        [System.Diagnostics.CodeAnalysis.StringSyntax("regex")]
        #endif
        public const string RegexString = @"""BEGIN:VCARD(?:\r\n(FN=(?<FormattedName>\w+))?(?:\r\n(?<Version>VERSION(?<VersionVersion>(?<Majorint>\d+)\.(?<Minor>\d+))))?(?:
(?<Address>ADR;TYPE=(?<TypeAddressType>\w+):(?<StreetAddress>\w+);(?<Locality>\w+);(?<Region>\w+);(?<PostalCode>\w+);(?<Country>\w+)))?(?:
(?<Birthday>BDAY(?<BirthdayDateOnly>(?<Yearint>\d{4})-(?<Month>\d{2})-(?<Day>\d{2})))))?
END:VCARD""";

        #if NET7_0_OR_GREATER
        [GeneratedRegex(RegexString, RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline | RegexOptions.RightToLeft | RegexOptions.Singleline)]
        public static partial System.Text.RegularExpressions.Regex Regex();
        #else
        private static readonly System.Text.RegularExpressions.Regex _regex = new System.Text.RegularExpressions.Regex(RegexString);
        public static System.Text.RegularExpressions.Regex Regex() => _regex;
        #endif



        public VCard () { }

        public VCard (string s)
        {
            var match = Regex().Match(s);
            if (!match.Success)
            {
                throw new System.ArgumentException($"The string \"{s}\" does not match the regular expression \"{RegexString}\".", nameof(s));
            }

            FormattedName = (string)System.Convert.ChangeType(match.Groups["FormattedName"]?.Value, typeof(string));
            Version = (string)System.Convert.ChangeType(match.Groups["Version"]?.Value, typeof(string));
            Minor = (int)System.Convert.ChangeType(match.Groups["Minor"]?.Value, typeof(int));
            Address = (string)System.Convert.ChangeType(match.Groups["Address"]?.Value, typeof(string));
            StreetAddress = (string)System.Convert.ChangeType(match.Groups["StreetAddress"]?.Value, typeof(string));
            Locality = (string)System.Convert.ChangeType(match.Groups["Locality"]?.Value, typeof(string));
            Region = (string)System.Convert.ChangeType(match.Groups["Region"]?.Value, typeof(string));
            PostalCode = (int)System.Convert.ChangeType(match.Groups["PostalCode"]?.Value, typeof(int));
            Country = (string)System.Convert.ChangeType(match.Groups["Country"]?.Value, typeof(string));
            Birthday = (string)System.Convert.ChangeType(match.Groups["Birthday"]?.Value, typeof(string));
            Month = (int)System.Convert.ChangeType(match.Groups["Month"]?.Value, typeof(int));
            Day = (int)System.Convert.ChangeType(match.Groups["Day"]?.Value, typeof(int));
        }
    }
}
/*
    baseType: System.Object
    baseTypeSymbolKind: Type
    baseTypeValue:
    baseTypeValueType:
    isClass: True
    targetDataStructureType: record class
    typeName: VCard
    namespaceName: Contacts
    matches: 22: (?<FormattedName>\w+)
25: FormattedName
0:
52: (?<Version>VERSION:(?<Version:Version>(?<Major:int>\d+)
55: Version
0:
109: (?<Minor:int>\d+)
112: Minor
118: int
135: (?<Address>ADR;TYPE=(?<Type:AddressType>\w+)
138: Address
0:
180: (?<StreetAddress>\w+)
183: StreetAddress
0:
202: (?<Locality>\w+)
205: Locality
0:
219: (?<Region>\w+)
222: Region
0:
234: (?<PostalCode:int>\w+)
237: PostalCode
248: int
257: (?<Country>\w+)
260: Country
0:
280: (?<Birthday>BDAY:(?<Birthday:DateOnly>(?<Year:int>\d{4})
283: Birthday
0:
337: (?<Month:int>\d{2})
340: Month
346: int
357: (?<Day:int>\d{2})
360: Day
364: int
    regex: BEGIN:VCARD(?:\r\n(FN=(?<FormattedName>\w+))?(?:\r\n(?<Version>VERSION:(?<Version:Version>(?<Major:int>\d+)\.(?<Minor:int>\d+))))?(?:
(?<Address>ADR;TYPE=(?<Type:AddressType>\w+):(?<StreetAddress>\w+);(?<Locality>\w+);(?<Region>\w+);(?<PostalCode:int>\w+);(?<Country>\w+)))?(?:
(?<Birthday>BDAY:(?<Birthday:DateOnly>(?<Year:int>\d{4})-(?<Month:int>\d{2})-(?<Day:int>\d{2}))))?
END:VCARD
    visibility: public
*/
