using System.Text.Json.Serialization;

namespace DepsAnalyzer.Model;

public class Target
{
    public Target(Dictionary<string, string>? dependencies)
    {
        Dependencies = dependencies;
    }
    
    // Key: PackageName e.g. "NuGet.CommandLine".
    // Value: Version string e.g. "3.6.0"
    //
    // Can be null, not all NuGet packages have dependencies
    public Dictionary<string, string>? Dependencies { get; }
    
    // runtime property exists but isn't used 
}