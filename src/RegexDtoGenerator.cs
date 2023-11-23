namespace Dgmjr.RegexDtoGenerator;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;

using Dgmjr.CodeGeneration.Logging;
using Dgmjr.RegexDtoGenerator.Models;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using static System.DateTimeOffset;
using static System.Text.RegularExpressions.RegexOptions;
using static Dgmjr.RegexDtoGenerator.Constants;

/// <summary>
/// The main class for the RegexDtoGenerator.
/// Implements the IIncrementalGenerator interface.
/// </summary>
[Generator]
public partial class RegexDtoGenerator : IIncrementalGenerator
{
    /// <summary>
    /// The regular expression string used to match named groups in a regular expression.
    /// </summary>
    private const string NewLine = "\n";

    /// <summary>
    /// The regular expression options used when matching named groups in a regular expression.
    /// </summary>
    private const string RegexString =
        @"\(\?\<(?<Name>[a-zA-Z0-9]+)(?:\:(?<Type>[a-zA-Z0-9]+\??))?\>.*?\)";

    private const RegexOptions RegexOptions = Compiled | IgnoreCase | Multiline;

#if NET7_0_OR_GREATER
    [GeneratedRegex(RegexString, RegexOptions)]
    private static partial Regex Regex();
#else
    private static Regex Regex() => _regex;

    private static readonly Regex _regex = new(RegexString, RegexOptions);
#endif

    /// <summary>
    /// The logger used for logging information during code generation.
    /// </summary>
    private SourceGeneratorLogger<RegexDtoGenerator> Logger { get; set; }

    /// <summary>
    /// Initializes the RegexDtoGenerator.
    /// Registers the generated source code for the RegexDtoAttribute and the generated source code for each RegexDto class.
    /// </summary>
    /// <param name="context">The IncrementalGeneratorInitializationContext.</param>
    ///
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(
            context =>
                context.AddSource(
                    RegexDtoAttributeFilename,
                    SourceText.From(RenderedRegexDtoAttributeDeclaration, Encoding.UTF8)
                )
        );

        using (
            Logger = new SourceGeneratorLogger<RegexDtoGenerator>(
                (message, severity) => WriteLine($"{severity}: {message}")
            )
        )
        {
            try
            {
                try
                {
                    var sources = context.SyntaxProvider.ForAttributeWithMetadataName(
                        RegexDtoAttributeName,
                        (node, _) => node is TypeDeclarationSyntax,
                        (ctx, _) =>
                        {
                            var sources = new List<RegexDtoFileModel>();
                            var attribute = ctx.Attributes.FirstOrDefault(
                                a => a.AttributeClass.Name == RegexDtoAttributeName
                            );
                            var regex = attribute?.ConstructorArguments
                                .FirstOrDefault()
                                .Value?.ToString();
                            if (!IsNullOrEmpty(regex))
                            {
                                var matches = Regex().Matches(regex);
                                var typeName = ctx.TargetSymbol.MetadataName;
                                var namespaceName =
                                    ctx.TargetSymbol.ContainingNamespace.ToDisplayString();
                                var targetDataStructureType = ctx.TargetNode.Kind() switch
                                {
                                    SyntaxKind.RecordStructDeclaration => "record struct",
                                    SyntaxKind.RecordDeclaration => "record class",
                                    SyntaxKind.StructDeclaration => "struct",
                                    SyntaxKind.ClassDeclaration => "class",
                                    _
                                        => throw new NotSupportedException(
                                            $"The type {ctx.TargetNode.GetType().Name} is not supported."
                                        )
                                };
                                var baseTypeSymbolKind = attribute?.ConstructorArguments
                                    .Skip(1)
                                    .FirstOrDefault()
                                    .Kind;
                                var baseTypeValue = attribute.ConstructorArguments
                                    .Skip(1)
                                    .FirstOrDefault()
                                    .Value;
                                var baseTypeValueType = baseTypeValue?.GetType();
                                var baseType = attribute?.ConstructorArguments
                                    .Skip(1)
                                    .FirstOrDefault()
                                    .Value?.ToString();
                                Logger.LogInformation($"Found regex: {regex}.");
                                Logger.LogInformation(
                                    "Base type symbol",
                                    additionalFields: new Dictionary<string, object>
                                    {
                                        ["Kind"] = baseTypeSymbolKind,
                                        ["Value"] = baseType
                                    }
                                );

                                var isClass =
                                    ctx.TargetNode.Kind()
                                        is SyntaxKind.ClassDeclaration
                                            or SyntaxKind.RecordDeclaration;
                                if (!isClass)
                                {
                                    baseType = null;
                                }
                                baseType ??= typeof(object).FullName;

                                Logger.LogInformation($"Found regex: {regex}.");
                                var visibility = ctx.TargetSymbol.DeclaredAccessibility switch
                                {
                                    Accessibility.Public => "public",
                                    Accessibility.Internal => "internal",
                                    Accessibility.Protected => "protected",
                                    Accessibility.ProtectedOrInternal => "protected internal",
                                    Accessibility.ProtectedAndInternal => "private protected",
                                    Accessibility.Private => "private",
                                    _
                                        => throw new NotSupportedException(
                                            $"The accessibility {ctx.TargetSymbol.DeclaredAccessibility} is not supported."
                                        )
                                };
                                Logger.LogInformation(
                                    "Parsed regex:.",
                                    additionalFields: new Dictionary<string, object>
                                    {
                                        ["Regex"] = regex,
                                        ["Visibility"] = visibility,
                                        ["TargetDataStructureType"] = targetDataStructureType,
                                        ["TypeName"] = typeName,
                                        ["NamespaceName"] = namespaceName,
                                        ["Matches"] = matches
                                            .OfType<Match>()
                                            .Select(
                                                m =>
                                                    new
                                                    {
                                                        Name = m.Groups["Name"].Value,
                                                        Type = m.Groups["Type"].Value,
                                                        IsNullable = m.Groups[
                                                            "Type"
                                                        ].Value.EndsWith("?")
                                                    }
                                            )
                                            .ToArray()
                                    }
                                );

                                var baseTypeDiagnosticInfo = $"""
                                /*
                                    baseType: {baseType}
                                    baseTypeSymbolKind: {baseTypeSymbolKind}
                                    baseTypeValue: {baseTypeValue}
                                    baseTypeValueType: {baseTypeValueType}
                                    isClass: {isClass}
                                    targetDataStructureType: {targetDataStructureType}
                                    typeName: {typeName}
                                    namespaceName: {namespaceName}
                                    matches: {Join(NewLine, matches.OfType<Match>().SelectMany(m => m?.Groups?.OfType<Group>()).Select(g => g?.Index + ": " + g?.Value))}
                                    regex: {regex}
                                    visibility: {visibility}
                                */
                                """;

                                if (isClass)
                                {
                                    Logger.LogInformation(
                                        "The target type is a class, so the properties will be virtual and there will be a base type inherited from."
                                    );

                                    var basePropertiedModel = new RegexDtoPropertiesDeclarationModel
                                    {
                                        TypeName = typeName,
                                        Properties = matches
                                            .OfType<Match>()
                                            .Select(
                                                m =>
                                                    new RegexDtoPropertyDeclarationModel(
                                                        Name: m.Groups["Name"].Value,
                                                        Type: IsNullOrEmpty(
                                                            m.Groups["Type"].Value.Replace("?", "")
                                                        )
                                                            ? "string"
                                                            : m.Groups["Type"].Value.Replace(
                                                                "?",
                                                                ""
                                                            ),
                                                        IsNullable: m.Groups["Type"].Value.Contains(
                                                            "?"
                                                        ),
                                                        Overridability: isClass ? "virtual" : "",
                                                        IsClass: isClass
                                                    )
                                            )
                                            .ToArray()
                                    };

                                    var baseTypeModel = new RegexDtoBaseTypeDeclarationModel(
                                        typeName,
                                        targetDataStructureType,
                                        namespaceName,
                                        Regex()
                                            .Replace(
                                                regex,
                                                m =>
                                                    m.Value.Replace(
                                                        ":" + m.Groups["Type"].Value,
                                                        ""
                                                    )
                                            ),
                                        new RegexDtoConstructorDeclarationModel(
                                            isClass ? @protected : @public,
                                            isClass ? @protected : @public,
                                            typeName + Base,
                                            basePropertiedModel.Properties
                                        ),
                                        basePropertiedModel.Properties,
                                        baseType
                                    ).Initialize();

                                    sources.Add(
                                        new(
                                            RenderRegexDtoBaseTypeDeclaration(baseTypeModel),
                                            baseTypeModel.TypeName + "Base",
                                            baseTypeModel.NamespaceName
                                        )
                                    );
                                }

                                var propertiesModel = new RegexDtoPropertiesDeclarationModel
                                {
                                    TypeName = typeName,
                                    Properties = matches
                                        .OfType<Match>()
                                        .Select(
                                            m =>
                                                new RegexDtoPropertyDeclarationModel(
                                                    Name: m.Groups["Name"].Value,
                                                    Type: IsNullOrEmpty(
                                                        m.Groups["Type"].Value.Replace("?", "")
                                                    )
                                                        ? "string"
                                                        : m.Groups["Type"].Value.Replace("?", ""),
                                                    IsNullable: m.Groups["Type"].Value.EndsWith(
                                                        "?"
                                                    ),
                                                    Overridability: isClass ? "override" : "",
                                                    IsClass: isClass
                                                )
                                        )
                                        .ToArray()
                                };

                                var typeModel = new RegexDtoDeclarationModel(
                                    typeName,
                                    targetDataStructureType,
                                    namespaceName,
                                    Regex()
                                        .Replace(
                                            regex,
                                            m => m.Value.Replace(":" + m.Groups["Type"].Value, "")
                                        ),
                                    new RegexDtoConstructorDeclarationModel(
                                        @public,
                                        @public,
                                        typeName,
                                        propertiesModel.Properties
                                    ),
                                    propertiesModel.Properties
                                ).Initialize();

                                sources.Add(
                                    new(
                                        RenderRegexDtoDeclaration(typeModel),
                                        typeModel.TypeName,
                                        typeModel.NamespaceName
                                    )
                                );
                                sources.Add(
                                    new(
                                        baseTypeDiagnosticInfo,
                                        $"{typeModel.TypeName}.{nameof(baseTypeDiagnosticInfo)}.g.cs",
                                        string.Empty
                                    )
                                );
                            }
                            return sources;
                        }
                    );

                    context.RegisterSourceOutput(
                        sources,
                        (context, sources) =>
                        {
                            foreach (var source in sources)
                            {
                                context.AddSource(
                                    $"{source.TypeName}.cs",
                                    SourceText.From(
                                        RenderHeader(source) + source.Source,
                                        Encoding.UTF8
                                    )
                                );
                            }
                        }
                    );
                }
                catch (Exception ex)
                {
                    context.RegisterPostInitializationOutput(
                        context =>
                            context.AddSource(
                                "Error.g.cs",
                                $"""
                    /*
                    {ex.Message}
                    {ex.StackTrace}
                    {ex.InnerException?.Message}
                    {ex.InnerException?.StackTrace}
                    {ex.InnerException?.InnerException?.Message}
                    {ex.InnerException?.InnerException?.StackTrace}
                    {ex.InnerException?.InnerException?.InnerException?.Message}
                    {ex.InnerException?.InnerException?.InnerException?.StackTrace}
                    */
                """
                            )
                    );
                }
            }
            catch (Exception ex)
            {
                context.RegisterPostInitializationOutput(
                    context =>
                        context.AddSource(
                            "Error.g.cs",
                            $"""
                                    /*
                                    {ex.Message}
                                    {ex.StackTrace}
                                    {ex.InnerException?.Message}
                                    {ex.InnerException?.StackTrace}
                                    {ex.InnerException?.InnerException?.Message}
                                    {ex.InnerException?.InnerException?.StackTrace}
                                    {ex.InnerException?.InnerException?.InnerException?.Message}
                                    {ex.InnerException?.InnerException?.InnerException?.StackTrace}
                                    */
                                """
                        )
                );
            }
        }
    }
}
