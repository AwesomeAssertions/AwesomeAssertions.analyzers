using Microsoft.CodeAnalysis;

namespace AwesomeAssertions.Analyzers.Tests;

/// <summary>
/// Struct that stores information about a Diagnostic appearing in a source
/// </summary>
public sealed class DiagnosticResult
{
    public DiagnosticResultLocation[] Locations
    {
        get => field ?? [];
        set;
    }

    public DiagnosticSeverity Severity { get; set; }

    public string Id { get; set; }

    public string Message { get; set; }

    public string Path => Locations.Length > 0 ? Locations[0].Path : "";

    public int Line => Locations.Length > 0 ? Locations[0].Line : -1;

    public int Column => Locations.Length > 0 ? Locations[0].Column : -1;

    public string VisitorName { get; set; }
}
