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

[Generator]
public partial class RegexDtoGenerator : IIncrementalGenerator
{
    private const string NewLine = "\n";
    private const string RegexString = @"\(\?\<(?<Name>[a-zA-Z]+)(?:\:(?<Type>[a-zA-Z]+\??))?\>.*?\)";
    private const RegexOptions RegexOptions = Compiled | IgnoreCase | Multiline;

#if NET7_0_OR_GREATER
    [GeneratedRegex(RegexString, RegexOptions)]
    private static partial REx Regex();
#else
    private static REx Regex() => _regex;
    private static readonly REx _regex = new(RegexString, RegexOptions);
#endif

    private SourceGeneratorLogger<RegexDtoGenerator> Logger { get; set; }

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        using (Logger = new SourceGeneratorLogger<RegexDtoGenerator>((message, severity) => WriteLine($"{severity}: {message}")))
        {
            try
            {
                context.RegisterPostInitializationOutput(
                    context =>
                        context.AddSource(
                            $"{RegexDtoAttributeName}.g.cs",
                            SourceText.From(
                                HeaderTemplate.Render(new
                                {
                                    FileName = $"{RegexDtoAttributeName}.g.cs",
                                    CreatedDate = UtcNow.ToString(DateFormat)
                                })
                                    +
                                    RegexDtoAttributeDeclarationTemplate.Render(new
                                    {
                                        FileName = $"{RegexDtoAttributeName}.g.cs",
                                        CreatedDate = UtcNow.ToString(DateFormat)
                                    }
                                ),
                                Encoding.UTF8
                            )
                        )
                    );

                try
                {
                    var sources = context.SyntaxProvider.ForAttributeWithMetadataName(
                        RegexDtoAttributeName,
                        (node, _) => node is TypeDeclarationSyntax,
                        (ctx, _) =>
                        {
                            var sources = new List<RegexDtoFileModel>();
                            var attribute = ctx.Attributes.FirstOrDefault(a => a.AttributeClass.Name == RegexDtoAttributeName);
                            var regex = attribute
                                ?.ConstructorArguments
                                .FirstOrDefault()
                                .Value
                                ?.ToString();
                            if (!IsNullOrEmpty(regex))
                            {
                                var matches = Regex().Matches(regex);
                                var typeName = ctx.TargetSymbol.MetadataName;
                                var namespaceName = ctx.TargetSymbol.ContainingNamespace.ToDisplayString();
                                var targetDataStructureType = ctx.TargetNode.Kind() switch
                                {
                                    SyntaxKind.RecordStructDeclaration => "record struct",
                                    SyntaxKind.RecordDeclaration => "record class",
                                    SyntaxKind.StructDeclaration => "struct",
                                    SyntaxKind.ClassDeclaration => "class",
                                    _ => throw new NotSupportedException($"The type {ctx.TargetNode.GetType().Name} is not supported.")
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
                                var baseType =
                                        attribute
                                            ?.ConstructorArguments
                                            .Skip(1)
                                            .FirstOrDefault()
                                            .Value
                                            ?.ToString();
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
                                    $"Parsed regex:.",
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
                                                        IsNullable = m.Groups["Type"].Value.EndsWith("?")
                                                    }
                                            )
                                            .ToArray()
                                    }
                                );

                                var baseTypeDiagnosticInfo =
                                $"""
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
                                        $"The target type is a class, so the properties will be virtual and there will be a base type inherited from."
                                    );

                                    var propertiesDeclarationModel =
                                        new RegexDtoPropertiesDeclarationModel
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
                                                            IsNullable: m.Groups["Type"].Value.EndsWith("?"),
                                                            Overridability: isClass ? "virtual" : "",
                                                            IsClass: isClass
                                                        )
                                                )
                                                .ToArray()
                                        };

                                    var baseTypeModel = new RegexDtoDeclarationTemplateModel
                                    {
                                        NamespaceName = namespaceName,
                                        TypeName = typeName + "Base",
                                        Visibility = visibility,
                                        TargetDataStructureType = targetDataStructureType,
                                        Regex = Regex().Replace(
                                            regex,
                                            m => m.Value.Replace(":" + m.Groups["Type"].Value, "")
                                        ),
                                        BaseType = !isClass || baseType != typeof(object).FullName
                                                ? $"{baseType}"
                                                : "",
                                        Members =
                                        $"""
                            { RegexDtoParseDeclarationTemplate.Render(propertiesDeclarationModel) }
                            { RegexDtoPropertiesDeclarationTemplate.Render(propertiesDeclarationModel) }
                            {
                                            RegexDtoConstructorDeclarationTemplate.Render(new RegexDtoConstructorDeclarationModel
                                            {
                                                ParameterlessConstructorVisibility = isClass ? "protected" : "public",
                                                ParameterizedConstructorVisibility = isClass ? "protected" : "public",
                                                TypeName = typeName + "Base",
                                                Properties = propertiesDeclarationModel.Properties
                                            })
                                        }
                            """
                                    };

                                    sources.Add(new(RegexDtoDeclarationTemplate.Render(baseTypeModel), baseTypeModel.TypeName, baseTypeModel.NamespaceName));
                                }

                                // if(isCla
                                var propertiesDeclarationModel2 =
                                    new RegexDtoPropertiesDeclarationModel
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
                                                        IsNullable: m.Groups["Type"].Value.EndsWith("?"),
                                                        Overridability: isClass ? "override" : "",
                                                        IsClass: isClass
                                                    )
                                            )
                                            .ToArray()
                                    };

                                var typeModel = new RegexDtoDeclarationTemplateModel
                                {
                                    NamespaceName = namespaceName,
                                    TypeName = typeName,
                                    Visibility = visibility,
                                    TargetDataStructureType = targetDataStructureType,
                                    Regex = Regex().Replace(
                                        regex,
                                        m => m.Value.Replace(":" + m.Groups["Type"].Value, "")
                                    ),
                                    BaseType = isClass ? typeName + "Base" : "",
                                    Members = $"""
                            { (!isClass ? RegexDtoParseDeclarationTemplate.Render(propertiesDeclarationModel2) : "") }
                            { (!isClass ? RegexDtoPropertiesDeclarationTemplate.Render(propertiesDeclarationModel2) : "") }
                            {
                                        RegexDtoConstructorDeclarationTemplate.Render(new RegexDtoConstructorDeclarationModel
                                        {
                                            ParameterlessConstructorVisibility = "public",
                                            ParameterizedConstructorVisibility = "public",
                                            TypeName = typeName,
                                            Properties = propertiesDeclarationModel2.Properties
                                        })
                                    }
                            """
                                };

                                sources.Add(new(RegexDtoDeclarationTemplate.Render(typeModel), typeModel.TypeName, typeModel.NamespaceName));
                                sources.Add(new(baseTypeDiagnosticInfo, $"{typeModel.TypeName}.{nameof(baseTypeDiagnosticInfo)}.g.cs", string.Empty));
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
                                context.AddSource($"{source.TypeName}.cs",
                                SourceText.From(HeaderTemplate.Render(new
                                {
                                    FileName = $"{source.TypeName}.cs",
                                    CreatedDate = UtcNow.ToString(DateFormat)
                                }) + source.Source,
                                Encoding.UTF8));
                            }
                        }
                    );

                    //     (attribute, metadata) =>
                    // {
                    //     var regex = attribute.ArgumentList.Arguments[0].Expression.ToString();
                    //     var matches = Regex.Matches(regex);
                    //     var className = metadata.Name;
                    //     var namespaceName = metadata.ContainingNamespace.ToDisplayString();
                    //     var source = $@"
                    //         using System;
                    //         using System.Text.RegularExpressions;
                    //         namespace {namespaceName}
                    //         {{
                    //             [RegexDto(""{regex}"")]
                    //             public class {className}
                    //             {{
                    //                 {string.Join(Environment.NewLine, matches.Select(m => $"public string {m.Groups[1].Value} {{ get; set; }}"))}
                    //             }}
                    //         }}
                    //     ";
                    //     context.AddSource($"{className}.cs", SourceText.From(source, Encoding.UTF8));
                    // });
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
