/*
 * RegexDtoPropertyDeclarationModel.cs
 *
 *   Created: 2023-02-01-02:41:46
 *   Modified: 2023-02-01-02:49:03
 *
 *   Author: David G. Mooore, Jr. <justin@Dgmjr.com>
 *
 *   Copyright © 2022-2023 David G. Mooore, Jr., All Rights Reserved
 *      License: MIT (https://opensource.org/licenses/MIT)
 */

namespace Dgmjr.RegexDtoGenerator.Models;

// public const string RegexDtoDeclaration = """"

// #nullable enable
// namespace {{~ namespace_name ~}};

// {{ visibility }} partial {{ target_data_structure_type }} {{ type_name }}
// {
// #if NET7_0_OR_GREATER
//     [System.Diagnostics.CodeAnalysis.StringSyntax("regex")]
// #endif
//     public const string RegexString = @"""{{ regex }}""";

// //#if NET7_0_OR_GREATER
// //    [GeneratedRegex(RegexString, RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline | RegexOptions.RightToLeft | RegexOptions.Singleline)]
// //    public static partial System.Text.RegularExpressions.Regex Regex();
// //#else
//     private static readonly System.Text.RegularExpressions.Regex _regex = new System.Text.RegularExpressions.Regex(RegexString, RegexOptions);
//     public static System.Text.RegularExpressions.Regex Regex() => _regex;
// //#endif

//     public static {{ type_name }} Parse(string s)
//     {
//         var match = Regex().Match(s);
//         if (!match.Success)
//         {
//             throw new System.ArgumentException($"The string \"{s}\" does not match the regular expression \"{RegexString}\".", nameof(s));
//         }

//         return new {{ type_name }}
//         {
//             {{~ for property in properties ~}}
//             {{~ if property.is_nullable ~}}
//             {{ property.name }} = match.Groups["{{ property.name }}"]?.Value is null ? null : ({{ property.type }}?)System.Convert.ChangeType(match.Groups["{{ property.name }}"]?.Value, typeof({{ property.type }})),
//             {{~ else ~}}
//             {{ property.name }} = ({{ property.type }})System.Convert.ChangeType(match.Groups["{{ property.name }}"]?.Value, typeof({{ property.type }})),
//             {{~ end ~}}
//             {{~ end ~}}
//         };
//     }

//     {{~ for property in properties ~}}
//     {{~ if property.is_nullable ~}}
//     public {{ property.overridability }} {{ property.type }}? {{ property.name }} { get; set; }
//     {{~ else ~}}
//     public {{ property.overridability }} {{ property.type }} {{ property.name }} { get; set; }
//     {{~ end ~}}
//     {{~ end ~}}
// }
// """";

internal record struct RegexDtoPropertyDeclarationModel(
    string Name,
    string Type,
    bool IsNullable,
    string Overridability,
    bool IsClass = false
);
