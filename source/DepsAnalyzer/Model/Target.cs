namespace DepsAnalyzer.Model;

public class Target
{
    // Key: PackageName e.g. "NuGet.CommandLine".
    // Value: Version string e.g. "3.6.0"
    public required Dictionary<string, string> Dependencies { get; init; }
    
    // runtime property exists but isn't used 
}