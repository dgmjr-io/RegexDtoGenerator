using System;

namespace Dgmjr.RegexDtoGenerator.Models;

public record struct RegexDtoFileModel(string Source, string? TypeName = null, string? NamespaceName = null, string? CreatedDate = null)
{
    public string CreatedDate { get; } = CreatedDate ?? DateTimeOffset.UtcNow.ToString(Constants.DateFormat);
}
