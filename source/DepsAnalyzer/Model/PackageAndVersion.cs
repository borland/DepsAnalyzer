namespace DepsAnalyzer.Model;

public readonly struct PackageAndVersion
{
    public required string Package { get; init; }
    public required string Version { get; init; }
}