using DepsAnalyzer.Model;
using FluentAssertions;
using Xunit.Abstractions;

namespace DepsAnalyzer.Tests;
using static Utils;

public class DependencyGraphFilterTests
{
    private readonly ITestOutputHelper testOutputHelper;

    public DependencyGraphFilterTests(ITestOutputHelper testOutputHelper)
    {
        this.testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void FiltersSimpleGraph()
    {
        var thirdPartyA = DN("ThirdParty.PackageA", "2.0");
        var thirdPartyB = DN("ThirdParty.PackageB", "7.5");
        
        var myLibrary = DN("My.Library", "1.1", thirdPartyA, thirdPartyB);
        var myMain = DN("My.Main", "1.0", thirdPartyA, myLibrary);

        var root = DN("Root", "0", myMain);

        // act
        var filtered = DependencyGraph.Filter(root, "My.Library");

        // testOutputHelper.WriteLine(MermaidDiagram.Generate(filtered, omitRootNode:true));
        
        var expected = DN("Root", "0",
            myLibrary);

        filtered.Should().BeEquivalentTo(expected);
    }
}