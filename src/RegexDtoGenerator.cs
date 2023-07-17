namespace Dgmjr.RegexDtoGenerator;

using System;
using System.Collections.Generic;
using System.Linq;
using Dgmjr.CodeGeneration.Logging;
using Dgmjr.RegexDtoGenerator.Models;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

[Generator]
public class RegexDtoGenerator : IIncrementalGenerator
{
    private const string NewLine = "\n";
    private const string RegexString = @"\(\?\<(?<Name>\w+)(?:\:(?<Type>\w+\??))?\>.*?\)";
    private static readonly REx Regex =
        new(RegexString, Compiled | IgnoreCase | Multiline);

    private SourceGeneratorLogger<RegexDtoGenerator> Logger { get; } = new SourceGeneratorLogger<RegexDtoGenerator>((message, severity) => WriteLine($"{severity}: {message}"));

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        try
        {
            var sources = context.SyntaxProvider.ForAttributeWithMetadataName(
                Constants.RegexDtoAttributeName,
                (node, _) => node is TypeDeclarationSyntax,
                (ctx, _) =>
                {
                    var regex = ctx.Attributes[0].ConstructorArguments
                        .FirstOrDefault()
                        .Value.ToString();
                    var matches = Regex.Matches(regex);
                    var typeName = ctx.TargetSymbol.MetadataName;
                    var namespaceName = ctx.TargetSymbol.ContainingNamespace.ToDisplayString();
                    var targetDataStructureType =
                        ctx.TargetNode.Kind() is SyntaxKind.RecordStructDeclaration
                            ? "record struct"
                            : ctx.TargetNode.Kind() is SyntaxKind.RecordDeclaration
                                ? "record class"
                                : ctx.TargetNode.Kind() is SyntaxKind.StructDeclaration
                                    ? "struct"
                                    : ctx.TargetNode.Kind() is SyntaxKind.ClassDeclaration
                                        ? "class"
                                        : throw new NotSupportedException(
                                            $"The type {ctx.TargetNode.GetType().Name} is not supported."
                                        );
                    var baseTypeSymbolKind = ctx.Attributes[0].ConstructorArguments
                        .Skip(1)
                        .FirstOrDefault()
                        .Kind;
                    var baseTypeValue = ctx.Attributes[0].ConstructorArguments
                                        .Skip(1)
                                        .FirstOrDefault()
                                        .Value;
                    var baseTypeValueType = baseTypeValue?.GetType();
                    var baseType =
                            ctx.Attributes[0].ConstructorArguments
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
                        matches: {string.Join(NewLine, matches.OfType<Match>().SelectMany(m => m?.Groups?.OfType<Group>()).Select(g => g?.Index + ": " + g?.Value))}
                        regex: {regex}
                        visibility: {visibility}
                    */
                    """;

                    var source = new System.Text.StringBuilder();

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
                                                Type: string.IsNullOrEmpty(
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
                            Regex = Regex.Replace(
                                regex,
                                m => m.Value.Replace(":" + m.Groups["Type"].Value, "")
                            ),
                            BaseType = !isClass || baseType != typeof(object).FullName
                                    ? $"{baseType}"
                                    : "",
                            Members = $"""
                                {Constants.RegexDtoParseDeclarationTemplate.Render(propertiesDeclarationModel)}
                                {Constants.RegexDtoPropertiesDeclarationTemplate.Render(propertiesDeclarationModel)}
                                {Constants.RegexDtoConstructorDeclarationTemplate.Render(new RegexDtoConstructorDeclarationModel
                            {
                                ParameterlessConstructorVisibility = isClass ? "protected" : "public",
                                ParameterizedConstructorVisibility = isClass ? "protected" : "public",
                                TypeName = typeName + "Base",
                                Properties = propertiesDeclarationModel.Properties
                            })}
                                """
                        };

                        var __ = source.Append(Constants.RegexDtoDeclarationTemplate.Render(baseTypeModel));
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
                        Regex = Regex.Replace(
                            regex,
                            m => m.Value.Replace(":" + m.Groups["Type"].Value, "")
                        ),
                        BaseType = isClass ? typeName + "Base" : "",
                        Members = $"""
                            {(!isClass ? Constants.RegexDtoParseDeclarationTemplate.Render(propertiesDeclarationModel2) : "")}
                            {(!isClass ? Constants.RegexDtoPropertiesDeclarationTemplate.Render(propertiesDeclarationModel2) : "")}
                            {Constants.RegexDtoConstructorDeclarationTemplate.Render(new RegexDtoConstructorDeclarationModel
                        {
                            ParameterlessConstructorVisibility = "public",
                            ParameterizedConstructorVisibility = "public",
                            TypeName = typeName,
                            Properties = propertiesDeclarationModel2.Properties
                        })}
                            """
                    };

                    var ___ = source.Append(Constants.RegexDtoDeclarationTemplate.Render(typeModel))
                                     .Append(baseTypeDiagnosticInfo);

                    return new
                    {
                        Source = source.ToString(),
                        TypeName = typeName,
                        NamespaceName = namespaceName
                    };
                }
            );

            context.RegisterSourceOutput(
                sources,
                (context, source) => context.AddSource($"{source.TypeName}.cs",
                Scriban.Template.Parse(Constants.Header).Render(new
                {
                    FileName = $"{source.TypeName}.cs",
                    CreatedDate = Now.ToString("yyyy-MM-dd")
                }) + source.Source)
            );

            context.RegisterPostInitializationOutput(
                context =>
                    context.AddSource(
                        $"{Constants.RegexDtoAttributeName}.g.cs",
                        Constants.RegexDtoAttributeDeclarationTemplate.Render(new
                        {
                            Filename = $"{Constants.RegexDtoAttributeName}.g.cs",

                        })
                    )
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
}
