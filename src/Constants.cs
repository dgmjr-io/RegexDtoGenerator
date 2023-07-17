using System.Security.AccessControl;
/*
 * Constants.cs
 *
 *   Created: 2022-12-28-04:55:15
 *   Modified: 2022-12-28-04:55:17
 *
 *   Author: David G. Mooore, Jr. <david@dgmjr.io>
 *
 *   Copyright © 2022-2023 David G. Mooore, Jr., All Rights Reserved
 *      License: MIT (https://opensource.org/licenses/MIT)
 */

namespace Dgmjr.RegexDtoGenerator;

public static partial class Constants
{
    public const string RegexDtoGenerator = "RegexDtoGenerator";
    public const string RegexDtoGeneratorVersion = "0.0.1";
    public const string RegexDtoGeneratorDescription =
        "Generates a C# type from a regular expression.";
    public const string RegexDtoGeneratorHelpText =
        "Generates a C# type from a regular expression.";
    public const string RegexDtoGeneratorHelpTextExample =
        "Example: RegexDtoGenerator -r \"(?<Name>\\w+) (?<Age>\\d+)\" -n Person";
    public const string RegexDtoGeneratorHelpTextExample2 =
        "Example: RegexDtoGenerator -r \"(?<Name>\\w+) (?<Age>\\d+)\" -n Person -o \"C:\\Users\\Justin\\Documents\\Person.cs\"";
    public const string RegexDtoGeneratorHelpTextExample3 =
        "Example: RegexDtoGenerator -r \"(?<Name>\\w+) (?<Age>\\d+)\" -n Person -o \"C:\\Users\\Justin\\Documents\\Person.cs\" -p \"Dgmjr.erator\"";
    public const string RegexDtoGeneratorHelpTextExample4 =
        "Example: RegexDtoGenerator -r \"(?<Name>\\w+) (?<Age>\\d+)\" -n Person -o \"C:\\Users\\Justin\\Documents\\Person.cs\" -p \"Dgmjr.erator\" -c \"PersonDto\"";
    public const string RegexDtoGeneratorHelpTextExample5 =
        "Example: RegexDtoGenerator -r \"(?<Name>\\w+) (?<Age>\\d+)\" -n Person -o \"C:\\Users\\Justin\\Documents\\Person.cs\" -p \"Dgmjr.erator\" -c \"PersonDto\" -i";
    public const string RegexDtoGeneratorHelpTextExample6 =
        "Example: RegexDtoGenerator -r \"(?<Name>\\w+) (?<Age>\\d+)\" -n Person -o \"C:\\Users\\Justin\\Documents\\Person.cs\" -p \"Dgmjr.erator\" -c \"PersonDto\" -i -s";

    public const string RegexDtoAttributeName = "RegexDtoAttribute";

    public const string ThisAssemblyTitle = ThisAssembly.Info.Title;

    public const string Header =
    $$$$""""
    /*
     * <auto-generated />
     * This file was generated by "{{{{ThisAssembly.Info.Title}}}}".
     * {{ file_name }}
     *
     *   Created: {{ created_date }}
     */
    using System.Text.RegularExpressions;
    using System.Collections.Generic;
    using System.Linq;
    using System;
    using System.Text;
    using System.Globalization;
    using System.Diagnostics.CodeAnalysis;
    using static System.Text.RegularExpressions.RegexOptions;

    #nullable enable

    """";

    public const string RegexDtoAttributeDeclaration =
    $$$""""

    {{{Header}}}

    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
    [System.CodeDom.Compiler.GeneratedCode("{{{ThisAssembly.Info.Title}}}", "{{{ThisAssembly.Info.Version}}}")]
    internal sealed class RegexDtoAttribute : System.Attribute
    {
        public RegexDtoAttribute(
    #if NET7_0_OR_GREATER
                [System.Diagnostics.CodeAnalysis.StringSyntax("regex")]
    #endif
                string regex, System.Type? baseType = null, RegexOptions regexOptions = RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline)
        {
            Regex = regex;
        }

        public string Regex { get; }
    }
    """";

    public const string RegexDtoAttributeUsage =
    """"
    [RegexDto(@""(?< Name >\\w +)(?< Age >\\d +)"")]
    public class PersonDto
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
    """";

    public const string RegexDtoBaseTypeDeclaration =
    """"

    #nullable enable

    namespace {{ namespace_name }}
    {
        [System.CodeDom.Compiler.GeneratedCode("{{{{ThisAssembly.Info.Title}}}}", "{{{{ThisAssembly.Info.Version}}}}")]
        {{ visibility }}
        partial abstract
        {{ target_data_structure_type }}
        {{ type_name }}
        Base
        {{ if base_type != "" }} : {{ base_type }}
        {{ end }}
        {
            const RegexOptions RegexOptions = (RegexOptions)({{ regex_options | string.replace ","  " | " }});

            #if NET7_0_OR_GREATER
                    [System.Diagnostics.CodeAnalysis.StringSyntax("regex")]
        #endif
        public const string RegexString = @"""{{ regex }}""";
        //#if NET7_0_OR_GREATER
        //        [GeneratedRegex(RegexString, RegexOptions)]
        //        public static partial System.Text.RegularExpressions.Regex Regex();
        //#else
        private static readonly System.Text.RegularExpressions.Regex _regex = new System.Text.RegularExpressions.Regex(RegexString, RegexOptions);
        public static System.Text.RegularExpressions.Regex Regex() => _regex;
        //#endif

        {{ members }}
                }

                protected
        {{ type_name }}
        Base()
                {
        }
        }

    """";

    public const string RegexDtoTypeDeclaration =
    $$$$""""

    #nullable enable

    namespace {{ namespace_name }}
        {
        [System.CodeDom.Compiler.GeneratedCode("{{{{ThisAssembly.Info.Title}}}}", "{{{{ThisAssembly.Info.Version}}}}")]
        {{ visibility }}
        partial {{ target_data_structure_type }}
        {{ type_name }}
        {{ if base_type != "" }} : {{ base_type }}
        {{ end }}
        {
            const RegexOptions RegexOptions = (RegexOptions)({{ regex_options | string.replace "," " | " }});

        #if NET7_0_OR_GREATER
                [System.Diagnostics.CodeAnalysis.StringSyntax("regex")]
    #endif
    public const string RegexString = @"{{ regex }}";

    //#if NET7_0_OR_GREATER
    //        [GeneratedRegex(RegexString, RegexOptions.Compiled | RegexOptions)]
    //        public static partial System.Text.RegularExpressions.Regex Regex();
    //#else
    private static readonly System.Text.RegularExpressions.Regex _regex = new System.Text.RegularExpressions.Regex(RegexString, RegexOptions);
    public static System.Text.RegularExpressions.Regex Regex() => _regex;
    //#endif

    {{ members }}
            }
        }

    """";

    public const string RegexDtoParseDeclaration =
    """
    public static
    {{ type_name }}
    Parse(string s)
    {
        var match = Regex().Match(s);
        if (!match.Success)
        {
            throw new System.ArgumentException($"The string \"{s}\" does not match the regular expression \"{RegexString}\".", nameof(s));
        }

        return new {{ type_name }}
        {
            {{~ for property in properties ~}}
            {{~ if property.is_nullable ~}}
            {{ property.name }} = match.Groups["{{ property.name }}"]?.Value is null ? null : ({{ property.type }}?)System.Convert.ChangeType(match.Groups["{{ property.name }}"]?.Value, typeof({{ property.type }})),
                {{~ else ~}}
            {{ property.name }} = ({{ property.type }})System.Convert.ChangeType(match.Groups["{{ property.name }}"]?.Value, typeof({{ property.type }})),
                {{~ end ~}}
            {{~ end ~}}
        };
    }
    """;
    public const string RegexDtoConstructorDeclaration =
    """
    {{ parameterless_constructor_visibility }}
    {{ type_name }} () { }

    {{ parameterized_constructor_visibility }}
    {{ type_name }} (string s)
    {
        var match = Regex().Match(s);
        if (!match.Success)
        {
            throw new System.ArgumentException($"The string \"{s}\" does not match the regular expression \"{RegexString}\".", nameof(s));
        }

        {{~ for property in properties ~}}
        {{~ if property.is_nullable ~}}
        {{ property.name }} = match.Groups["{{ property.name }}"]?.Value is null ? null : ({{ property.type }}?)System.Convert.ChangeType(match.Groups["{{ property.name }}"]?.Value, typeof({{ property.type }}));
        {{~ else ~}}
        {{ property.name }} = ({{ property.type }})System.Convert.ChangeType(match.Groups["{{ property.name }}"]?.Value, typeof({{ property.type }}));
        {{~ end ~}}
        {{~ end ~}}
    }
    """;

    public const string RegexDtoPropertiesDeclaration =
    """
    {{~ for property in properties ~}}
    {{~ if property.is_nullable ~}}
    public
    {{ property.overridability }}
    {{ property.type }}?
    {{ property.name }}
    { get; set; }
    {{~ else ~}}
    public
    {{ property.overridability }}
    {{ property.type }}
    {{ property.name }}
    { get; set; }
    {{~end ~}}
    {{~end ~}}
    """;

    public static readonly Scriban.Template RegexDtoDeclarationTemplate =
        Scriban.Template.Parse(RegexDtoTypeDeclaration, nameof(RegexDtoTypeDeclaration)
    );
    public static readonly Scriban.Template RegexDtoParseDeclarationTemplate =
        Scriban.Template.Parse(RegexDtoParseDeclaration, nameof(RegexDtoParseDeclaration));
    public static readonly Scriban.Template RegexDtoPropertiesDeclarationTemplate =
        Scriban.Template.Parse(RegexDtoPropertiesDeclaration, nameof(RegexDtoPropertiesDeclaration));
    public static readonly Scriban.Template RegexDtoConstructorDeclarationTemplate =
        Scriban.Template.Parse(RegexDtoConstructorDeclaration, nameof(RegexDtoConstructorDeclaration));

    public static readonly Scriban.Template RegexDtoAttributeDeclarationTemplate =
        Scriban.Template.Parse(RegexDtoAttributeDeclaration, nameof(RegexDtoAttributeDeclaration));
}
