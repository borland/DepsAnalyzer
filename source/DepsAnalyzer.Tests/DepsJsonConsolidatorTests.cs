using DepsAnalyzer.Model;
using FluentAssertions;

namespace DepsAnalyzer.Tests;

public class DepsJsonConsolidatorTests
{
    [Fact]
    public void ConsolidatesPackagesWithTheSamePrefixByVersion()
    {
        var doc = new DepsJsonDocument
        {
            Targets = new Dictionary<NetFrameworkMoniker, Dictionary<PackageAndVersion, Target>>
            {
                [new NetFrameworkMoniker(".NETCoreApp,Version=v6.0")] = new()
                {
                    [new PackageAndVersion("ScratchConsoleApp", "1.0.0")] = new(new Dictionary<string, string>
                    {
                        ["Newtonsoft.Json"] = "13.0.1",
                        ["System.Runtime"] = "6.0.0",
                        ["System.Collections"] = "6.0.0",
                        ["System.Management"] = "6.0.0",
                        ["System.SomeExtension"] = "7.1.0",
                        ["System.SomeOtherExtension"] = "7.1.0",
                    }),
                    [new PackageAndVersion("Newtonsoft.Json", "13.0.1")] = new(new Dictionary<string, string>
                    {
                        ["System.Runtime"] = "6.0.0",
                        ["System.Management"] = "6.0.0",
                        ["System.SomeExtension"] = "7.1.0",
                    }),
                }
            }
        };
        
        DepsJsonConsolidator.Consolidate(doc, ConsolidateBy.PrefixAndVersion, new List<(string Prefix, string ConsolidateInto)>
        {
            ("System.", "SystemGROUP")
        });
        
        doc.Should().BeEquivalentTo(new DepsJsonDocument
        {
            Targets = new Dictionary<NetFrameworkMoniker, Dictionary<PackageAndVersion, Target>>
            {
                [new NetFrameworkMoniker(".NETCoreApp,Version=v6.0")] = new()
                {
                    [new PackageAndVersion("ScratchConsoleApp", "1.0.0")] = new(new Dictionary<string, string>
                    {
                        ["Newtonsoft.Json"] = "13.0.1",
                        ["SystemGROUP_6.0.0"] = "6.0.0",
                        ["SystemGROUP_7.1.0"] = "7.1.0",
                    }),
                    [new PackageAndVersion("Newtonsoft.Json", "13.0.1")] = new(new Dictionary<string, string>
                    {
                        ["SystemGROUP_6.0.0"] = "6.0.0",
                        ["SystemGROUP_7.1.0"] = "7.1.0",
                    }),
                }
            }
        });
    }
    
    [Fact]
    public void ConsolidatesPackagesThatHaveFurtherDependenciesByPrefixAndVersion()
    {
        var doc = new DepsJsonDocument
        {
            Targets = new Dictionary<NetFrameworkMoniker, Dictionary<PackageAndVersion, Target>>
            {
                [new NetFrameworkMoniker(".NETCoreApp,Version=v6.0")] = new()
                {
                    [new PackageAndVersion("ScratchConsoleApp", "1.0.0")] = new(new Dictionary<string, string>
                    {
                        ["Newtonsoft.Json"] = "13.0.1",
                        ["Newtonsoft.Json.Extensions"] = "13.0.1",
                        ["System.Runtime"] = "6.0.0",
                    }),
                    [new PackageAndVersion("Newtonsoft.Json", "13.0.1")] = new(new Dictionary<string, string>
                    {
                        ["System.Runtime"] = "6.0.0",
                    }),
                }
            }
        };
        
        DepsJsonConsolidator.Consolidate(doc, ConsolidateBy.PrefixAndVersion, new List<(string Prefix, string ConsolidateInto)>
        {
            ("Newtonsoft.", "NewtonsoftGROUP")
        });
        
        doc.Should().BeEquivalentTo(new DepsJsonDocument
        {
            Targets = new Dictionary<NetFrameworkMoniker, Dictionary<PackageAndVersion, Target>>
            {
                [new NetFrameworkMoniker(".NETCoreApp,Version=v6.0")] = new()
                {
                    [new PackageAndVersion("ScratchConsoleApp", "1.0.0")] = new(new Dictionary<string, string>
                    {
                        ["NewtonsoftGROUP_13.0.1"] = "13.0.1",
                        ["System.Runtime"] = "6.0.0",
                    }),
                    [new PackageAndVersion("NewtonsoftGROUP_13.0.1", "13.0.1")] = new(new Dictionary<string, string>
                    {
                        ["System.Runtime"] = "6.0.0",
                    }),
                }
            }
        });
    }
    
    [Fact]
    public void ConsolidatesPackagesWithTheSamePrefixByPrefixOnly()
    {
        var doc = new DepsJsonDocument
        {
            Targets = new Dictionary<NetFrameworkMoniker, Dictionary<PackageAndVersion, Target>>
            {
                [new NetFrameworkMoniker(".NETCoreApp,Version=v6.0")] = new()
                {
                    [new PackageAndVersion("ScratchConsoleApp", "1.0.0")] = new(new Dictionary<string, string>
                    {
                        ["Newtonsoft.Json"] = "13.0.1",
                        ["System.Runtime"] = "6.0.0",
                        ["System.Collections"] = "6.0.0",
                        ["System.Management"] = "6.0.0",
                        ["System.SomeExtension"] = "7.1.0",
                        ["System.SomeOtherExtension"] = "7.1.0",
                    }),
                    [new PackageAndVersion("Newtonsoft.Json", "13.0.1")] = new(new Dictionary<string, string>
                    {
                        ["System.Runtime"] = "6.0.0",
                        ["System.Management"] = "6.0.0",
                        ["System.SomeExtension"] = "7.1.0",
                    }),
                }
            }
        };
        
        DepsJsonConsolidator.Consolidate(doc, ConsolidateBy.PrefixOnly, new List<(string Prefix, string ConsolidateInto)>
        {
            ("System.", "SystemGROUP")
        });
        
        doc.Should().BeEquivalentTo(new DepsJsonDocument
        {
            Targets = new Dictionary<NetFrameworkMoniker, Dictionary<PackageAndVersion, Target>>
            {
                [new NetFrameworkMoniker(".NETCoreApp,Version=v6.0")] = new()
                {
                    [new PackageAndVersion("ScratchConsoleApp", "1.0.0")] = new(new Dictionary<string, string>
                    {
                        ["Newtonsoft.Json"] = "13.0.1",
                        ["SystemGROUP"] = "multiple", // version range
                    }),
                    [new PackageAndVersion("Newtonsoft.Json", "13.0.1")] = new(new Dictionary<string, string>
                    {
                        ["SystemGROUP"] = "multiple", // version range
                    }),
                }
            }
        });
    }
}