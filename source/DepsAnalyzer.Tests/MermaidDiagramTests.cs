using FluentAssertions;

namespace DepsAnalyzer.Tests;
using static Utils;

public class MermaidDiagramTests
{
    [Fact]
    public static void DrawsSimpleExample()
    {
        // build the expected graph:
        var thirdPartyA = DN("ThirdPartyPackageA", "2.0");
        var thirdPartyB = DN("ThirdPartyPackageB", "7.5");
        
        var myLibrary = DN("MyLibraryPackage", "1.1", thirdPartyA, thirdPartyB);
        var myMain = DN("MyMainPackage", "1.0", thirdPartyA, myLibrary);

        var root = DN("Root", "0", myMain);

        var str = MermaidDiagram.Generate(root);
        str.Should().Be(@"flowchart LR
Root_0[""Root@0""]
MyMainPackage_1_0[""MyMainPackage@1.0""]
ThirdPartyPackageA_2_0[""ThirdPartyPackageA@2.0""]
MyLibraryPackage_1_1[""MyLibraryPackage@1.1""]
ThirdPartyPackageB_7_5[""ThirdPartyPackageB@7.5""]

Root_0 --> MyMainPackage_1_0
MyMainPackage_1_0 --> ThirdPartyPackageA_2_0
MyMainPackage_1_0 --> MyLibraryPackage_1_1
MyLibraryPackage_1_1 --> ThirdPartyPackageA_2_0
MyLibraryPackage_1_1 --> ThirdPartyPackageB_7_5
");
    }
    
    [Fact]
    public static void DrawsSimpleExampleOmittingRootNode()
    {
        // build the expected graph:
        var thirdPartyA = DN("ThirdPartyPackageA", "2.0");
        var thirdPartyB = DN("ThirdPartyPackageB", "7.5");
        
        var myLibrary = DN("MyLibraryPackage", "1.1", thirdPartyA, thirdPartyB);
        var myMain = DN("MyMainPackage", "1.0", thirdPartyA, myLibrary);

        var root = DN("Root", "0", myMain);

        var str = MermaidDiagram.Generate(root, omitRootNode: true);
        str.Should().Be(@"flowchart LR
MyMainPackage_1_0[""MyMainPackage@1.0""]
ThirdPartyPackageA_2_0[""ThirdPartyPackageA@2.0""]
MyLibraryPackage_1_1[""MyLibraryPackage@1.1""]
ThirdPartyPackageB_7_5[""ThirdPartyPackageB@7.5""]

MyMainPackage_1_0 --> ThirdPartyPackageA_2_0
MyMainPackage_1_0 --> MyLibraryPackage_1_1
MyLibraryPackage_1_1 --> ThirdPartyPackageA_2_0
MyLibraryPackage_1_1 --> ThirdPartyPackageB_7_5
");
    }
}