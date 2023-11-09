using DepsAnalyzer.Model;

namespace DepsAnalyzer;

public class DependencyGraph
{
    public static DependencyNode Build(Dictionary<PackageAndVersion, Target> dependencyInfo)
    {
        var root = new DependencyNode(new PackageAndVersion("Root", "0"));

        Merge(root, dependencyInfo);

        return root;
    }

    static void Merge(DependencyNode parent, Dictionary<PackageAndVersion, Target> dependencyInfo)
    {
        foreach (var (pv, target) in dependencyInfo)
        {
            var node = FindNode(parent, pv);
            if (node == null)
            {
                node = new DependencyNode(pv);
                parent.Dependencies.Add(pv, node);
            }

            if (target.Dependencies is not { } targetDependencies) continue;

            // horribly inefficient because this is the wrong shape
            var mapped = targetDependencies.Select(kv => new PackageAndVersion(kv.Key, kv.Value)).ToDictionary(x => x, _ => new Target(null));

            Merge(node, mapped);
        }
    }

    // Find an existing node for this package+version that we can attach onto; breadth-first search
    static DependencyNode? FindNode(DependencyNode parent, PackageAndVersion package)
    {
        if (parent.Package == package) return parent;
        if (parent.Dependencies.TryGetValue(package, out var child)) return child;

        foreach (var child2 in parent.Dependencies.Values)
        {
            if (FindNode(child2, package) is { } found) return found;
        }

        return null;
    }
}

public class DependencyNode
{
    public DependencyNode(PackageAndVersion package) : this(package, new())
    {
    }

    public DependencyNode(PackageAndVersion package, Dictionary<PackageAndVersion, DependencyNode> dependencies)
    {
        Package = package;
        Dependencies = dependencies;
    }

    public PackageAndVersion Package { get; }

    public Dictionary<PackageAndVersion, DependencyNode> Dependencies { get; }
}