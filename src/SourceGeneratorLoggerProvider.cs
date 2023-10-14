#define LOG
using System.Runtime.CompilerServices;

/*
 * SourceGeneratorLoggerProvider.cs
 *
 *   Created: 2022-12-24-01:58:25
 *   Modified: 2022-12-24-01:58:26
 *
 *   Author: David G. Moore, Jr. <david@dgmjr.io>
 *
 *   Copyright © 2022-2023 David G. Moore, Jr., All Rights Reserved
 *      License: MIT (https://opensource.org/licenses/MIT)
 */
namespace Dgmjr.CodeGeneration.Logging;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using static System.Convert;
using static System.String;
using static System.Text.Encoding;
using static System.Text.Json.JsonSerializer;

using AddSource = Action<string, string>; //string hintName, string sourceText);

public class SourceGeneratorLogger<TSourceGenerator>(AddSource addSource) : IDisposable
    where TSourceGenerator : IIncrementalGenerator
{
    private static string Filename =>
        UtcNow.ToString("hh-mm-ss") + "-" + guid.NewGuid().ToString().Substring(0, 2) + ".cs";
private readonly MemoryStream _ms = new();
private readonly JsonWriterOptions _options = new() { Indented = true, SkipValidation = true };
private Utf8JsonWriter _writer;
private Utf8JsonWriter Writer => _writer ??= new Utf8JsonWriter(_ms, _options);
protected AddSource AddSource { get; } = addSource;

protected virtual Utf8JsonWriter OpenWriter() => Writer;

[Conditional("LOG")]
public virtual void Log(
    string message,
    string severity = "Information",
    Exception? ex = null,
    [CallerFilePath] string? source = null,
    [CallerLineNumber] int? line = null,
    [CallerMemberName] string? memberName = null,
    IDictionary<string, object>? additionalFields = null,
    params object[] args
)
{
    var writer = OpenWriter();
    writer.WriteStartObject();
    writer.WriteString("id", $"{typeof(TSourceGenerator).Name}.{severity}");
    writer.WriteString("message", Format(message, args));
    writer.WriteString("severity", severity);
    foreach (var additionalField in additionalFields ?? new Dictionary<string, object>())
    {
        writer.WritePropertyName(additionalField.Key);
        Serialize(writer, additionalField.Value);
    }
    if (ex != null)
    {
        writer.WriteStartObject("exception");
        writer.WriteString("message", ex.Message);
        writer.WriteStartArray("stackTrace");
        _ = ex.StackTrace
            .Split('\n')
            .Select(frame =>
            {
                writer.WriteStringValue(frame);
                return true;
            });
        writer.WriteEndArray();
        writer.WriteEndObject();
    }
    if (source != null)
    {
        writer.WriteStartObject("location");
        writer.WriteString("path", Format("{0}({1},{2})", source, line, 0));
        writer.WriteString("member", memberName);
        writer.WriteNumber("line", ToDecimal(line));
        writer.WriteEndObject();
    }
    writer.WriteEndObject();
    writer.Flush();
    // var json = UTF8.GetString(ms.ToArray());
    // AddSource(Filename,
    // $"""
    // /*
    //     {json}
    // */
    // """
    // );
}

[Conditional("LOG")]
public virtual void LogError(
    string message,
    [CallerFilePath] string? source = null,
    [CallerLineNumber] int? line = null,
    [CallerMemberName] string? memberName = null,
    IDictionary<string, object>? additionalFields = null,
    params object[] args
) =>
    Log(
        message,
        severity: "Error",
        source: source,
        line: line,
        memberName: memberName,
        additionalFields: additionalFields,
        args: args
    );

[Conditional("LOG")]
public virtual void LogInformation(
    string message,
    [CallerFilePath] string? source = null,
    [CallerLineNumber] int? line = null,
    [CallerMemberName] string? memberName = null,
    IDictionary<string, object>? additionalFields = null,
    params object[] args
) =>
    Log(
        message,
        severity: "Information",
        source: source,
        line: line,
        memberName: memberName,
        additionalFields: additionalFields,
        args: args
    );

[Conditional("LOG")]
public virtual void LogError(
    Exception exception,
    [CallerFilePath] string? source = null,
    [CallerLineNumber] int? line = null,
    [CallerMemberName] string? memberName = null,
    IDictionary<string, object>? additionalFields = null,
    params object[] args
) =>
    Log(
        exception.Message,
        severity: "Error",
        source: source,
        line: line,
        memberName: memberName,
        additionalFields: additionalFields,
        args: args
    );

public void Dispose()
{
    Dispose(true);
    GC.SuppressFinalize(this);
}

protected virtual void Dispose(bool disposing)
{
    if (disposing)
    {
        Log("Finished!", severity: "Information");
        OpenWriter().Dispose();
        AddSource(Filename, UTF8.GetString(_ms.ToArray()));
    }

    // Free unmanaged resources
}
}
