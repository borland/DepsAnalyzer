namespace DepsAnalyzer.Model;

public class DepsJsonDocument
{
    // runtimeTarget property exists but isn't used
    
    // compilationOptions property exists but isn't used

    // Targets in the JSON looks like this:
    // "targets": {
    //     ".NETCoreApp,Version=v6.0": { // we parse this string into NetFrameworkMoniker
    //         "Octopus.Server/1.0.0": { // we parse this string into ExecutableAndVersion
    public required Dictionary<NetFrameworkMoniker, Dictionary<PackageAndVersion, Target>> Targets { get; init; }
}