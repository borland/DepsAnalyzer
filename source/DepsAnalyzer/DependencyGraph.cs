using DepsAnalyzer.Model;

namespace DepsAnalyzer;

public static class DependencyGraph
{
    public static DependencyNode Build(Dictionary<PackageAndVersion, Target> dependencyInfo)
    {
        var root = new DependencyNode(new PackageAndVersion("Root", "0"));

        Merge(root, root, dependencyInfo);

        return root;
    }

    static void Merge(DependencyNode root, DependencyNode parent, Dictionary<PackageAndVersion, Target> dependencyInfo)
    {
        foreach (var (pv, target) in dependencyInfo)
        {
            var (node, foundOnOtherParent) = FindNode(root, pv); // important: When we find we need to always start from the root in case the dependency is down another branch
            if (node == null)
            {
                node ??= new DependencyNode(pv);
            }
            else
            {
                if (foundOnOtherParent == root)
                {
                    // special-case: We found an existing node ON the root. Re-home it to this node; the root should only contain things that have no inbound references
                    root.Dependencies.Remove(pv);
                }
            }
            
            // if we found it on another parent and that wasn't the root, add it there.
            // only put things on the root if we can't find them elsewhere
            // else we add it to the root because there's nowhere else to put it (it might get re-homed later)
            if(parent != root || foundOnOtherParent == null) parent.Dependencies.Add(pv, node);


            if (target.Dependencies is not { } targetDependencies) continue;

            // horribly inefficient because this is the wrong shape
            var mapped = targetDependencies.Select(kv => new PackageAndVersion(kv.Key, kv.Value)).ToDictionary(x => x, _ => new Target(null));

            Merge(root, node, mapped);
        }
    }

    // Find an existing node for this package+version that we can attach onto; breadth-first search
    static (DependencyNode? FoundNode, DependencyNode? FoundNodeParent) FindNode(DependencyNode parent, PackageAndVersion package)
    {
        if (parent.Package == package) return (parent, null);
        if (parent.Dependencies.TryGetValue(package, out var child)) return (child, parent);

        foreach (var child2 in parent.Dependencies.Values)
        {
            if (FindNode(child2, package) is { } found) return found;
        }

        return (null, null);
    }

    // TODO a test
    public static DependencyNode Filter(DependencyNode graph, string filterString)
    {
        var filteredRoot = new DependencyNode(new PackageAndVersion("Root", "0"));

        static void AddFilteredTo(DependencyNode addTo, DependencyNode startFrom, string filterString)
        {
            foreach (var (pv, node) in startFrom.Dependencies)
            {
                if (pv.Package.StartsWith(filterString))
                {
                    addTo.Dependencies.TryAdd(pv, node);
                }
                
                // which way we recurse shouldn't matter; the given package/version should only appear once in the graph
                // but we have to recurse because we don't know at what depth it is
                AddFilteredTo(addTo, node, filterString);
            }
        }

        AddFilteredTo(filteredRoot, graph, filterString);

        return filteredRoot;
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

    public override string ToString() => $"{Package} ({Dependencies.Count} dependencies)";
}