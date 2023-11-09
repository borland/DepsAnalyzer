namespace DepsAnalyzer.Model;

public record PackageAndVersion(string Package, string Version)
{
    public override string ToString() => $"{Package}@{Version}";
}
