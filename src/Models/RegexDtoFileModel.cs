using System;

namespace Dgmjr.RegexDtoGenerator.Models;

internal record struct RegexDtoFileModel(
    string Source,
    string? TypeName = null,
    string? NamespaceName = null,
    string? CreatedDate = null
)
{
    public string CreatedDate { get; } = CreatedDate ?? UtcNow.ToString(Constants.DateFormat);
}
