/*
 * RegexDtoDeclarationTemplateModel.cs
 *
 *   Created: 2023-02-01-02:41:30
 *   Modified: 2023-02-01-02:47:31
 *
 *   Author: David G. Moore, Jr. <david@dgmjr.io>
 *
 *   Copyright © 2022-2023 David G. Moore, Jr., All Rights Reserved
 *      License: MIT (https://opensource.org/licenses/MIT)
 */

namespace Dgmjr.RegexDtoGenerator;

using Dgmjr.RegexDtoGenerator.Models;
using static System.Text.RegularExpressions.RegexOptions;
using static Dgmjr.RegexDtoGenerator.Constants;

internal record struct RegexDtoDeclarationModel
{
    public RegexDtoDeclarationModel(
        string typeName,
        string targetDataStructureType,
        string namespaceName,
        string regex,
        RegexDtoConstructorDeclarationModel constructor,
        IEnumerable<RegexDtoPropertyDeclarationModel> properties,
        string baseType = "",
        Rxo regexOptions = Compiled | IgnoreCase | ExplicitCapture
    )
    {
        TypeName = typeName;
        TargetDataStructureType = targetDataStructureType;
        NamespaceName = namespaceName;
        Regex = regex;
        Constructor = constructor;
        Properties = properties;
        BaseType = IsClass ? (TypeName + Base) : "";
        RegexOptions = regexOptions;
    }

    public RegexDtoDeclarationModel Initialize()
    {
        ParseDeclaration = $$$"""
        /// <summary>Parses <paramref name="s" /> into an instance of <see cref="{{{TypeName}}}" /></summary>
        /// <param name="s">The <see langword="string" /> to parse</param>
        /// <returns>The parsed <see cref="{{{TypeName}}}">DTO<see></returns>
    public static {{{TypeName
}
}} Parse(string s)
        {
    return new {{ { TypeName} }
} (s);
        }
        """;
        PropertiesDeclarations = Join("\n", Properties.Select(p => p.Declaration));
ConstructorDeclaration = Constructor.Declaration;
return this;
    }

    public const string Visibility = "public";
public readonly Rxo RegexOptions { get; init; }
public readonly string NamespaceName { get; init; }
public readonly string Regex { get; init; }
public readonly string TypeName { get; init; }
public readonly RegexDtoConstructorDeclarationModel Constructor { get; init; }
public readonly IEnumerable<RegexDtoPropertyDeclarationModel> Properties { get; init; }
public readonly string TargetDataStructureType { get; init; }
public readonly bool IsClass => TargetDataStructureType.Contains(Constants.Class);

public string ConstructorDeclaration { get; private set; }
public string PropertiesDeclarations { get; private set; }
public string ParseDeclaration { get; private set; }
public readonly string BaseType { get; init; }
}

internal record struct RegexDtoBaseTypeDeclarationModel
{
    public RegexDtoBaseTypeDeclarationModel(
        string typeName,
        string targetDataStructureType,
        string namespaceName,
        string regex,
        RegexDtoConstructorDeclarationModel constructor,
        IEnumerable<RegexDtoPropertyDeclarationModel> properties,
        string baseType = "",
        Rxo regexOptions = Compiled | IgnoreCase | ExplicitCapture
    )
    {
        TypeName = typeName;
        TargetDataStructureType = targetDataStructureType;
        NamespaceName = namespaceName;
        Regex = regex;
        Constructor = constructor;
        Properties = properties;
        BaseType = IsClass ? (baseType ?? "") : "";
        RegexOptions = regexOptions;
    }

    public RegexDtoBaseTypeDeclarationModel Initialize()
    {
        ParseDeclaration = $$$"""
        /// <summary>Parses <paramref name="s" /> into an instance of <see cref="{{{TypeName}}}" /></summary>
        /// <param name="s">The <see langword="string" /> to parse</param>
        /// <returns>The parsed <see cref="{{{TypeName}}}">DTO<see></returns>
    public static {{{TypeName
}}} Parse(string s)
        {
    return new {{ { TypeName} }
} (s);
        }
        """;
        PropertiesDeclarations = Join("\n", Properties.Select(p => p.Declaration));
ConstructorDeclaration = Constructor.Declaration;
return this;
    }

    public const string Visibility = "public";
public readonly Rxo RegexOptions { get; init; }
public readonly string NamespaceName { get; init; }
public readonly string Regex { get; init; }
public readonly string TypeName { get; init; }
public readonly RegexDtoConstructorDeclarationModel Constructor { get; init; }
public readonly IEnumerable<RegexDtoPropertyDeclarationModel> Properties { get; init; }
public readonly string TargetDataStructureType { get; init; }
public readonly bool IsClass => TargetDataStructureType.Contains(Constants.Class);

public string ConstructorDeclaration { get; private set; }
public string PropertiesDeclarations { get; private set; }
public string ParseDeclaration { get; private set; }
public readonly string BaseType { get; init; }
}
