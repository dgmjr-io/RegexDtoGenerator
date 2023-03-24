#define LOG
using System.Runtime.CompilerServices;

/*
 * SourceGeneratorLoggerProvider.cs
 *
 *   Created: 2022-12-24-01:58:25
 *   Modified: 2022-12-24-01:58:26
 *
 *   Author: David G. Mooore, Jr. <david@dgmjr.io>
 *
 *   Copyright © 2022-2023 David G. Mooore, Jr., All Rights Reserved
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

public delegate void AddSource(string hintName, string sourceText);

public class SourceGeneratorLogger<TSourceGenerator> : IDisposable
{
    private string Filename => /*Path.Combine(_filename ,*/
        UtcNow.ToString("hh-mm-ss")
        + "-"
        + guid.NewGuid().ToString().Substring(0, 2)
        + ".cs"; //);
    private readonly MemoryStream _ms = new();
    private readonly JsonWriterOptions _options = new() { Indented = true, SkipValidation = true };
    private Utf8JsonWriter _writer;
    private Utf8JsonWriter Writer => _writer ??= new Utf8JsonWriter(_ms, _options);

    public SourceGeneratorLogger(AddSource addSource) => AddSource = (_, _) => { };

    protected AddSource AddSource { get; }

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

    public virtual void Dispose()
    {
        // using var @lock = Mutex.TryOpenExisting(Filename, out var mutex) ? mutex : new Mutex(false, Filename);
        Log("Finished!", severity: "Information");
        OpenWriter().Dispose();
        AddSource(Filename, UTF8.GetString(_ms.ToArray()));
    }
}

// public class SourceGeneratorLoggerProvider : ILoggerProvider
// {
//     private readonly SourceGeneratorContext _context;
//     private readonly string _sourceGeneratorName;

//     public SourceGeneratorLoggerProvider(SourceGeneratorContext context, string sourceGeneratorName)
//     {
//         _context = context;
//         _sourceGeneratorName = sourceGeneratorName;
//     }

//     private List<ILogger> _createdLoggers = new List<ILogger>();

//     public ILogger CreateLogger<TCategory>()
//      where TCategory : ISourceGenerator
//         =>  CreateLogger(_sourceGeneratorName);
//     public ILogger CreateLogger(string categoryName)
//     {
//         var logger = new SourceGeneratorLogger(_context, categoryName);
//         _createdLoggers.Add(logger);
//         return logger;
//     }

//     public void Dispose()
//     {
//         foreach (var logger in _createdLoggers)
//         {
//             (logger as IDisposable)?.Dispose();
//         }
//     }
// }

// public class SourceGeneratorContext : IDisposable
// {
//     public SourceGeneratorContext(AddSource addSource, DiagnosticSeverity logLevel, string outputPath)
//     {
//         writer = new Utf8JsonWriter(File.OpenWrite(outputPath), new JsonWriterOptions { Indented = true, SkipValidation = true });
//         AddSource = addSource;
//         LogLevel = logLevel switch
//         {
//             DiagnosticSeverity.Error => LogLevel.Error,
//             DiagnosticSeverity.Warning => LogLevel.Warning,
//             DiagnosticSeverity.Info => LogLevel.Information,
//             DiagnosticSeverity.Hidden => LogLevel.Debug,
//             _ => LogLevel.Trace
//         };
//     }

//     private readonly Utf8JsonWriter? writer;
//     public AddSource AddSource { get; }
//     public LogLevel LogLevel { get; }

//     public void Dispose() => writer.Dispose();

//     public void ReportDiagnostic(Diagnostic diagnostic)
//     {
//         writer.WriteStartObject();
//         writer.WriteString("id", diagnostic.Id);
//         writer.WriteString("message", diagnostic.GetMessage());
//         writer.WriteString("severity", diagnostic.Severity.ToString());
//         if(diagnostic.Location.IsInSource)
//         {
//             writer.WriteStartObject("location");
//             writer.WriteString("path", diagnostic.Location.SourceTree?.FilePath);
//             writer.WriteNumber("line", diagnostic.Location.GetLineSpan().StartLinePosition.Line);
//             writer.WriteNumber("column", diagnostic.Location.GetLineSpan().StartLinePosition.Character);
//             writer.WriteEndObject();
//         }
//         writer.WriteEndObject();
//     }
// }

// public class SourceGeneratorLogger : ILogger, IDisposable
// {
//     private readonly SourceGeneratorContext _context;
//     private readonly string _sourceGeneratorName;
//     public SourceGeneratorLogger(SourceGeneratorContext context, string sourceGeneratorName)
//     {
//         _context = context;
//         _sourceGeneratorName = sourceGeneratorName;
//     }

//     public IDisposable BeginScope<TState>(TState state)
//     {
//         return NullScope.Instance;
//     }

//     public void Dispose() => _context.Dispose();

//     public bool IsEnabled(LogLevel logLevel)
//     {
//         return logLevel >= _context.LogLevel;
//     }

//     public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
//     {
//         if (IsEnabled(logLevel))
//         {
//             var stackTrace = new StackTrace();
//             var callerInfo = new
//             {
//                 Filename = stackTrace.GetFrame(1).GetFileName(),
//                 LineNumber = stackTrace.GetFrame(1).GetFileLineNumber(),
//                 ColumnNumber = stackTrace.GetFrame(1).GetFileColumnNumber(),
//                 MethodName = stackTrace.GetFrame(1).GetMethod().Name
//             };
//             var message = formatter(state, exception);
//             _context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor($"{_sourceGeneratorName}.{eventId.Id}", message, message, _sourceGeneratorName, logLevel switch
//             {
//                 LogLevel.Error => DiagnosticSeverity.Error,
//                 LogLevel.Warning => DiagnosticSeverity.Warning,
//                 LogLevel.Information => DiagnosticSeverity.Info,
//                 LogLevel.Debug => DiagnosticSeverity.Hidden,
//                 _ => DiagnosticSeverity.Hidden
//             }, true), Location.Create(callerInfo.Filename, TextSpan.FromBounds(callerInfo.LineNumber, callerInfo.ColumnNumber), new LinePositionSpan(new LinePosition(callerInfo.LineNumber, callerInfo.ColumnNumber), new LinePosition(callerInfo.LineNumber, callerInfo.ColumnNumber)))));
//         }
//     }
// }

// internal sealed class NullScope : IDisposable
// {
//     public static NullScope Instance { get; } = new NullScope();
//     private NullScope()
//     {
//     }

//     public void Dispose()
//     {
//     }
// }
