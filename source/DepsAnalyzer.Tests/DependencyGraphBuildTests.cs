using DepsAnalyzer.Model;
using FluentAssertions;
using Xunit.Abstractions;

namespace DepsAnalyzer.Tests;
using static Utils;

public class DependencyGraphBuildTests
{
    private readonly ITestOutputHelper testOutputHelper;

    public DependencyGraphBuildTests(ITestOutputHelper testOutputHelper)
    {
        this.testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void BuildsSimpleGraph()
    {
        var dependencyInfo = new Dictionary<PackageAndVersion, Target>
        {
            [new PackageAndVersion("My.Main", "1.0")] = new(new Dictionary<string, string>
            {
               ["ThirdParty.PackageA"] = "2.0",
               ["My.Library"] = "1.1",
            }),
            
            [new PackageAndVersion("My.Library", "1.1")] = new(new Dictionary<string, string>
            {
                ["ThirdParty.PackageA"] = "2.0",
                ["ThirdParty.PackageB"] = "7.5",
            }),
        };

        var graph = DependencyGraph.Build(dependencyInfo);
        testOutputHelper.WriteLine(MermaidDiagram.Generate(graph, omitRootNode: false));

        // build the expected graph:
        var thirdPartyA = DN("ThirdParty.PackageA", "2.0");
        var thirdPartyB = DN("ThirdParty.PackageB", "7.5");
        
        var myLibrary = DN("My.Library", "1.1", thirdPartyA, thirdPartyB);
        var myMain = DN("My.Main", "1.0", thirdPartyA, myLibrary);

        var expectedRoot = DN("Root", "0", myMain);
        
        graph.Should().BeEquivalentTo(expectedRoot);
    }
    
    [Fact]
    public void BuildsGraphWithDependencyDeclaredBeforeDependent()
    {
        // This is the same test above except the library comes first.
        // This represents the fact that JSON files are just text, and are processed in order,
        // and that order may be significant.
        //
        // this is a bit quirky: If we were to do this in something like Go or Swift,
        // the dictionary order would be essentially random each time, however while
        // C# dictionaries don't strictly define order, in practice they are are ordered (most of the time)
        // due to their internal linked-list kind of thing. If Microsoft ever chose to break this
        // it'd be a big deal and we'd need to revisit
        var dependencyInfo = new Dictionary<PackageAndVersion, Target>
        {
            [new PackageAndVersion("My.Library", "1.1")] = new(new Dictionary<string, string>
            {
                ["ThirdParty.PackageA"] = "2.0",
                ["ThirdParty.PackageB"] = "7.5",
            }),
            [new PackageAndVersion("My.Main", "1.0")] = new(new Dictionary<string, string>
            {
                ["ThirdParty.PackageA"] = "2.0",
                ["My.Library"] = "1.1",
            }),
        };

        var graph = DependencyGraph.Build(dependencyInfo);
        testOutputHelper.WriteLine(MermaidDiagram.Generate(graph, omitRootNode: false));
        
        // build the expected graph:
        var thirdPartyA = DN("ThirdParty.PackageA", "2.0");
        var thirdPartyB = DN("ThirdParty.PackageB", "7.5");
        
        var myLibrary = DN("My.Library", "1.1", thirdPartyA, thirdPartyB);
        var myMain = DN("My.Main", "1.0", thirdPartyA, myLibrary);

        var expectedRoot = DN("Root", "0", myMain);
        
        graph.Should().BeEquivalentTo(expectedRoot);
    }
}