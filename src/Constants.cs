/*
 * Constants.cs
 *
 *   Created: 2022-12-28-04:55:15
 *   Modified: 2022-12-28-04:55:17
 *
 *   Author: David G. Moore, Jr. <david@dgmjr.io>
 *
 *   Copyright © 2022-2023 David G. Moore, Jr., All Rights Reserved
 *      License: MIT (https://opensource.org/licenses/MIT)
 */

namespace Dgmjr.RegexDtoGenerator;

using Dgmjr.RegexDtoGenerator.Models;

using Scriban;

using static Scriban.Template;

internal static partial class Constants
{
    private const string _cs = ".cs";
    private const string _g = ".g";
    private const string _g_cs = _g + _cs;
    public const string @Class = "class";
    public const string Base = nameof(Base);
    public const string @protected = nameof(@protected);
    public const string @public = nameof(@public);
    public const string DateFormat = "yyyy-mm-ddTHH:mm:ss.ffffzzzZ";
    private const string RegexDtoGenerator = nameof(RegexDtoGenerator);
    private const string RegexDtoGeneratorVersion = ThisAssembly.Info.Version;
    public const string RegexDtoAttributeName = "RegexDtoAttribute";
    public const string RegexDtoAttributeFilename = RegexDtoAttributeName + _g_cs;
    private const string ThisAssemblyName = ThisAssembly.Project.AssemblyName;

    private const string Header = $$$$""""
        /*
        * <auto-generated>
        * {{ filename }}
        * This file was auto-generated by: {{{{ThisAssemblyName}}}}
        * Version: {{{{RegexDtoGeneratorVersion}}}}
        * Generated: {{ created_date }}.
        * Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
        * </auto-generated>
        */
        using static System.AttributeTargets;
        using static System.Text.RegularExpressions.RegexOptions;
        using System;
        using System.CodeDom.Compiler;
        using System.Collections.Generic;
        using System.Diagnostics.CodeAnalysis;
        using System.Globalization;
        using System.Linq;
        using System.Runtime.CompilerServices;
        using System.Text;
        using System.Text.RegularExpressions;

        #nullable enable

        """";

    private const string GeneratedCodeAttributesList =
        $"[GeneratedCode(\"{ThisAssemblyName}\", \"{RegexDtoGeneratorVersion}\"), CompilerGenerated]";

    private const string RegexDtoAttributeDeclaration = $$$""""
        [AttributeUsage(@Class | @Struct, Inherited = false, AllowMultiple = false)]
        {{{GeneratedCodeAttributesList}}}
        internal sealed class RegexDtoAttribute(
            #if NET7_0_OR_GREATER
            [@StringSyntax(StringSyntax.Regex)]
            #endif
            string Regex, System.Type? BaseType = null, RegexOptions RegexOptions = Compiled | CultureInvariant | ExplicitCapture | IgnoreCase | IgnorePatternWhitespace | Singleline) : System.Attribute
        {
            public string Regex { get; } = Regex;
            public System.Type? BaseType { get; } = BaseType;
            public RegexOptions RegexOptions { get; } = RegexOptions;
        }
        """";

    private const string RegexDtoBaseTypeDeclaration = $$$""""
        namespace {{ namespace_name }}
        {
            {{{GeneratedCodeAttributesList}}}
            public abstract partial {{ target_data_structure_type }} {{ type_name }}Base {{ if base_type != "" }} : {{ base_type ~}}{{ end }}
            {
                /// <summary>The default <see cref="RegexOptions" /></summary>
                /// <value>(RegexOptions)({{ regex_options | string.replace ","  " | " }})</value>
                const RegexOptions RegexOptions = (RegexOptions)({{ regex_options | string.replace ","  " | " }});

                #if NET7_0_OR_GREATER
                [@StringSyntax(StringSyntax.Regex)]
                #endif
                public const string RegexString = @"{{ regex }}";

                // #if NET7_0_OR_GREATER
                // [GeneratedRegex(RegexString, RegexOptions)]
                // public static partial Regex Regex();
                // #else
                private static readonly Regex _regex = new (RegexString, RegexOptions);
                /// <summary>The <see cref="Regex" /> that will be used to validate and recognize the DTO</summary>
                public static Regex Regex() => _regex;
                // #endif

                public {{ if is_class }}virtual{{ end }} string OriginalString { get; init; }

                {{ properties_declarations }}

                {{ parse_declaration }}

                {{ constructor_declaration }}
            }
        }
        """";

    private const string RegexDtoTypeDeclaration = $$$$""""
        namespace {{ namespace_name }}
        {
            {{{{GeneratedCodeAttributesList}}}}
            public partial {{ target_data_structure_type }} {{ type_name }} {{ if base_type != "" }} : {{ base_type ~}}{{ end }}
            {
                const RegexOptions RegexOptions = (RegexOptions)({{ regex_options | string.replace "," " | " }});

                /// <summary>The regex string that will be used to validate and recognize the DTO</summary>
                #if NET7_0_OR_GREATER
                [@StringSyntax(StringSyntax.Regex)]
                #endif
                public const string RegexString = @"{{ regex }}";

                // #if NET7_0_OR_GREATER
                // [GeneratedRegex(RegexString, RegexOptions)]
                // public static partial Regex Regex();
                // #else
                private static readonly Regex _regex = new (RegexString, RegexOptions);
                /// <summary>The <see cref="Regex" /> that will be used to validate and recognize the DTO</summary>
                public static {{ if base_type != "" }}new{{ end }} Regex Regex() => _regex;
                // #endif

                public {{ if is_class }}override{{ end }} string OriginalString { get; init; }

                {{ properties_declarations }}

                {{ parse_declaration }}

                {{ constructor_declaration }}
            }
        }
        """";

    private const string RegexDtoParseDeclaration = """
        /// <summary>Parses the <paramref name="s">string</paramref> into an instance of <see cref="{{ type_name }}">the DTO</see></summary>
        /// <param name="s">The <see langword="string" /> to parse</param>
        /// <returns>The parsed <see cref="{{ type_name }}">DTO<see></returns>
        public static {{ type_name }} Parse(string s)
        {
            return new {{ type_name }}(s);
        }
        """;

    private const string RegexDtoConstructorDeclaration = """
        /// <summary>Instantiates a new, empty <see cref="{{ type_name }}" /></summary>
        {{ parameterless_constructor_visibility }} {{ type_name }} () { }

        /// <summary>Instantiates a new <see cref="{{ type_name }}" /> and parses the <paramref name="s" /></summary>
        {{ parameterized_constructor_visibility }} {{ type_name }} (string s)
        {
            var match = Regex().Match(s);
            if (!match.Success)
            {
                throw new ArgumentException($"The string \"{s}\" does not match the regular expression \"{RegexString}\".", nameof(s));
            }

            {{~ for property in properties ~}}
            {{~ if property.is_nullable ~}}
            {{ property.name }} = string.IsNullOrEmpty(match.Groups["{{ property.name }}"]?.Value) ? null : ({{ property.type }}?)System.Convert.ChangeType(match.Groups["{{ property.name }}"]?.Value, typeof({{ property.type }}));
            {{~ else ~}}
            {{ property.name }} = ({{ property.type }})System.Convert.ChangeType(match.Groups["{{ property.name }}"]?.Value, typeof({{ property.type }}));
            {{~ end ~}}
            {{~ end ~}}

            OriginalString = s;
        }
        """;

    private const string RegexDtoPropertyDeclaration = """
        {{~ if is_nullable ~}}
        public {{ overridability }} {{ type }}? {{ name }} { get; set; }
        {{~ else ~}}
        public {{ overridability }} {{ type }} {{ name }} { get; set; }
        {{~end ~}}
        """;

    private const string RegexDtoPropertiesDeclaration = $$$"""
        {{~ for property in properties ~}}
        {{{RegexDtoPropertyDeclaration}}}
        {{~end ~}}
        """;

    private static readonly Template RegexDtoDeclarationTemplate = Parse(
        RegexDtoTypeDeclaration,
        nameof(RegexDtoTypeDeclaration)
    );

    private static readonly Template RegexDtoBaseTypeDeclarationTemplate = Parse(
        RegexDtoBaseTypeDeclaration,
        nameof(RegexDtoBaseTypeDeclaration)
    );

    private static readonly Template RegexDtoParseDeclarationTemplate = Parse(
        RegexDtoParseDeclaration,
        nameof(RegexDtoParseDeclaration)
    );

    private static readonly Template RegexDtoPropertiesDeclarationTemplate = Parse(
        RegexDtoPropertiesDeclaration,
        nameof(RegexDtoPropertiesDeclaration)
    );

    private static readonly Template RegexDtoConstructorDeclarationTemplate = Parse(
        RegexDtoConstructorDeclaration,
        nameof(RegexDtoConstructorDeclaration)
    );

    private static readonly Template RegexDtoAttributeDeclarationTemplate = Parse(
        RegexDtoAttributeDeclaration,
        nameof(RegexDtoAttributeDeclaration)
    );
    private static readonly Template RegexDtoPropertyDeclarationTemplate = Parse(
        RegexDtoPropertyDeclaration,
        nameof(RegexDtoPropertyDeclaration)
    );

    private static Template HeaderTemplate => Parse(Header, nameof(Header));

    public static string RenderedRegexDtoAttributeDeclaration =>
        HeaderTemplate.Render(new FilenameAndTimestamp(RegexDtoAttributeFilename))
        + RegexDtoAttributeDeclarationTemplate.Render();

    public static string RenderDtoConstructorDeclaration(
        RegexDtoConstructorDeclarationModel model
    ) => RegexDtoConstructorDeclarationTemplate.Render(model);

    public static string RenderRegexDtoPropertiesDeclaration(
        RegexDtoPropertiesDeclarationModel model
    ) => RegexDtoPropertiesDeclarationTemplate.Render(model);

    public static string RenderRegexDtoParseDeclaration(RegexDtoPropertiesDeclarationModel model) =>
        RegexDtoParseDeclarationTemplate.Render(model);

    public static string RenderRegexDtoDeclaration(RegexDtoDeclarationModel model) =>
        RegexDtoDeclarationTemplate.Render(model);

    public static string RenderHeader(RegexDtoFileModel model) => HeaderTemplate.Render(model);

    public static string RenderRegexDtoBaseTypeDeclaration(
        RegexDtoBaseTypeDeclarationModel model
    ) => RegexDtoBaseTypeDeclarationTemplate.Render(model);

    public static string RenderRegexDtoPropertyDeclaration(
        RegexDtoPropertyDeclarationModel model
    ) => RegexDtoPropertyDeclarationTemplate.Render(model);
}
