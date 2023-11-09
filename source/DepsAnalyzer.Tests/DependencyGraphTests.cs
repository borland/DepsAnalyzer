using DepsAnalyzer.Model;
using FluentAssertions;
namespace DepsAnalyzer.Tests;
using static Utils;

public class DependencyGraphTests
{
    [Fact]
    public void BuildsSimpleGraph()
    {
        var dependencyInfo = new Dictionary<PackageAndVersion, Target>
        {
            [new PackageAndVersion("MyMainPackage", "1.0")] = new(new Dictionary<string, string>
            {
               ["ThirdPartyPackageA"] = "2.0",
               ["MyLibraryPackage"] = "1.1",
            }),
            
            [new PackageAndVersion("MyLibraryPackage", "1.1")] = new(new Dictionary<string, string>
            {
                ["ThirdPartyPackageA"] = "2.0",
                ["ThirdPartyPackageB"] = "7.5",
            }),
        };

        var graph = DependencyGraph.Build(dependencyInfo);

        // build the expected graph:
        var thirdPartyA = DN("ThirdPartyPackageA", "2.0");
        var thirdPartyB = DN("ThirdPartyPackageB", "7.5");
        
        var myLibrary = DN("MyLibraryPackage", "1.1", thirdPartyA, thirdPartyB);
        var myMain = DN("MyMainPackage", "1.0", thirdPartyA, myLibrary);

        var root = DN("Root", "0", myMain);
        
        graph.Should().BeEquivalentTo(root);
    }
    
    // TODO we need a test (and the supporting code) to make the graph work when we encounter myLibrary BEFORE the parent.
    // we need to "re-root" the tree.
}