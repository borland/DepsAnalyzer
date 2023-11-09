using DepsAnalyzer.Model;

namespace DepsAnalyzer.Tests;

public static class Utils
{
    // convenience helper to build the expected graph shape otherwise it's too clunky

    public static DependencyNode DN(string packageName, string packageVersion, params DependencyNode[] dependencies)
    {
        var node = new DependencyNode(new PackageAndVersion(packageName, packageVersion));
        foreach (var dep in dependencies)
        {
            node.Dependencies.Add(dep.Package, dep);
        }

        return node;
    }
}