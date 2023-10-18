namespace Dgmjr.RegexDtoGenerator;

using Microsoft.CodeAnalysis;

internal static class RegisterPostInitializationOutputExtensions
{
    public static void RegisterPostInitializationOutput(
        this IncrementalGeneratorInitializationContext context,
        string filename,
        string output
    )
    {
        context.RegisterPostInitializationOutput(ctx => ctx.AddSource(filename, output));
    }
}
