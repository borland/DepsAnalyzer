using System.ComponentModel.DataAnnotations.Schema;
using DepsAnalyzer.Model;

namespace DepsAnalyzer;

public enum ConsolidateBy
{
    /// <summary>
    /// If there are packages with the same prefix but different versions, they will be placed in different groups
    /// </summary>
    PrefixAndVersion,
    
    /// <summary>
    /// If there are packages with the same prefix but different versions, they will be placed in the same group
    /// </summary>
    PrefixOnly
}

// consolidate before building the node-graph as it's easier to work with a flat structure
public static class DepsJsonConsolidator
{
    // mutates the document because it's easy. Probably better to return a new one
    public static void Consolidate(DepsJsonDocument document, ConsolidateBy consolidateBy, List<(string Prefix, string ConsolidateInto)> prefixes)
    {
        foreach (var (f, v) in document.Targets)
        {
            Consolidate(v, consolidateBy, prefixes);
        }
    }
    
    static void Consolidate(Dictionary<PackageAndVersion, Target> dependencies, ConsolidateBy consolidateBy, List<(string Prefix, string ConsolidateInto)> prefixes)
    {
        Dictionary<PackageAndVersion, Target> groups = new();
        
        foreach (var (pv, target) in dependencies)
        {
            if(target.Dependencies is null) continue;

            foreach (var (package, version) in target.Dependencies.ToArray())
            {
                foreach (var (prefix, groupName) in prefixes)
                {
                    if (package.StartsWith(prefix))
                    {
                        var qualifiedGroupName = GetQualifiedGroupName(consolidateBy, groupName, version);
                        var groupPv = GetGroupPackageAndVersion(consolidateBy, qualifiedGroupName, version);
                        
                        if (!groups.TryGetValue(groupPv, out var groupTarget))
                        {
                            groupTarget = new Target(new Dictionary<string, string>());
                            groups.Add(groupPv, groupTarget);
                        }

                        // replace the package with our fake group package
                        target.Dependencies.Remove(package);
                        target.Dependencies.TryAdd(qualifiedGroupName, version); // no-op if it's already there
                    }
                }
            }
        }

        foreach (var (pv, target) in dependencies.ToArray())
        {
            foreach (var (prefix, groupName) in prefixes)
            {
                if (pv.Package.StartsWith(prefix))
                {
                    var qualifiedGroupName = GetQualifiedGroupName(consolidateBy, groupName, pv.Version);
                    var groupPv = GetGroupPackageAndVersion(consolidateBy, qualifiedGroupName, pv.Version);
                    
                    dependencies.Remove(pv);
                    if (!groups.TryGetValue(groupPv, out var replaceWithGroup)) throw new ArgumentException($"Can't find replacement group {replaceWithGroup}");

                    dependencies.TryAdd(groupPv, replaceWithGroup); // no-op if it's already there
                    
                    // now merge in all the dependencies this had (no-op if they're already there)
                    if (target.Dependencies is null) continue;

                    foreach (var (dependentPackage, version) in target.Dependencies)
                    {
                        // Dependencies should always be there because we created it further up
                        replaceWithGroup.Dependencies?.TryAdd(dependentPackage, version);
                    }
                }
            }
        }
    }

    // create the fake package representing the consolidated group
    // the fake group name needs to include the version or we can end up with "System.Runtime" v6.0 and "System.Management" v8.0
    // both trying to be called "System" and colliding on dictionary key
    static string GetQualifiedGroupName(ConsolidateBy consolidateBy, string groupName, string version) => consolidateBy switch
    {
        ConsolidateBy.PrefixAndVersion => $"{groupName}_{version}",
        ConsolidateBy.PrefixOnly => groupName,
        _ => throw new ArgumentException($"Unexpected consolidateBy {consolidateBy}")
    };
    
    static PackageAndVersion GetGroupPackageAndVersion(ConsolidateBy consolidateBy, string groupName, string version) => consolidateBy switch
    {
        ConsolidateBy.PrefixAndVersion => new PackageAndVersion(groupName, version),
        ConsolidateBy.PrefixOnly => new PackageAndVersion(groupName, "multiple"),
        _ => throw new ArgumentException($"Unexpected consolidateBy {consolidateBy}")
    };
}
