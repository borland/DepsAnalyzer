using DepsAnalyzer.Model;
using FluentAssertions;

namespace DepsAnalyzer.Tests;

public class Tests
{
    [Fact]
    public void ParsesSimpleDocument()
    {
        var parsed = DepsJsonParser.Parse(ScratchConsoleAppDepsJson);
        parsed.Should().BeEquivalentTo(new DepsJsonDocument
        {
            Targets = new Dictionary<NetFrameworkMoniker, Dictionary<PackageAndVersion, Target>>
            {
                [new NetFrameworkMoniker(".NETCoreApp,Version=v6.0")] = new()
                {
                    [new PackageAndVersion("ScratchConsoleApp", "1.0.0")] = new(new Dictionary<string, string>
                    {
                        ["Newtonsoft.Json"] = "13.0.1"
                    }),
                    [new PackageAndVersion("Newtonsoft.Json", "13.0.1")] = new(null),
                }
            }
        });
    }
    
    [Fact]
    public void ParsesDocumentWithMultipleDependencies()
    {
        var parsed = DepsJsonParser.Parse(MultiDepsJson);
        parsed.Should().BeEquivalentTo(new DepsJsonDocument
        {
            Targets = new Dictionary<NetFrameworkMoniker, Dictionary<PackageAndVersion, Target>>
            {
                [new NetFrameworkMoniker(".NETCoreApp,Version=v6.0")] = new()
                {
                    [new PackageAndVersion("ScratchConsoleApp", "1.0.0")] = new(new Dictionary<string, string>
                    {
                        ["AsyncFixer"] = "1.6.0",
                        ["Octopus.Core"] = "1.0.0",
                        ["Octopus.Migrator.Core"] = "1.0.0",
                        ["Octopus.Nevermore.Analyzers"] = "21.0.1644",
                        ["Octopus.RoslynAnalyzers"] = "1.3.880",
                        ["Octopus.Server.Extensibility.Web"] = "1.0.0",
                        ["Octopus.Shared"] = "1.0.0",
                        ["Octopus.UnwantedMethodCallsAnalyzer"] = "0.2.440",
                        ["Serilog.Extensions.Logging"] = "3.1.0",
                        ["StyleCop.Analyzers"] = "1.0.2",
                        ["step-validation"] = "2.0.2",
                    }),
                    [new PackageAndVersion("Octopus.Server.Extensibility.Authentication.OpenIDConnect.Common", "1.0.0")] = new(new Dictionary<string, string>
                    {
                        ["Microsoft.CSharp"] = "4.7.0",
                        ["Octopus.Configuration"] = "1.0.0",
                        ["Octopus.Data"] = "1.0.0",
                        ["Octopus.Diagnostics"] = "1.0.0",
                        ["Octopus.Server.Extensibility"] = "1.0.0",
                        ["Octopus.Server.Extensibility.Authentication"] = "1.0.0",
                        ["Octopus.Server.Extensibility.Web"] = "1.0.0",
                        ["Octopus.Time"] = "1.0.0",
                        ["System.IdentityModel.Tokens.Jwt"] = "6.27.0",
                    }),
                }
            }
        }, options => options.AllowingInfiniteRecursion());
    }

    // This is a real deps json copied verbatim from a trivial console app that used Newtonsoft.Json
    static ReadOnlySpan<byte> ScratchConsoleAppDepsJson => @"
{
  ""runtimeTarget"": {
    ""name"": "".NETCoreApp,Version=v6.0"",
    ""signature"": """"
  },
  ""compilationOptions"": {},
  ""targets"": {
    "".NETCoreApp,Version=v6.0"": {
      ""ScratchConsoleApp/1.0.0"": {
        ""dependencies"": {
          ""Newtonsoft.Json"": ""13.0.1""
        },
        ""runtime"": {
          ""ScratchConsoleApp.dll"": {}
        }
      },
      ""Newtonsoft.Json/13.0.1"": {
        ""runtime"": {
          ""lib/netstandard2.0/Newtonsoft.Json.dll"": {
            ""assemblyVersion"": ""13.0.0.0"",
            ""fileVersion"": ""13.0.1.25517""
          }
        }
      }
    }
  },
  ""libraries"": {
    ""ScratchConsoleApp/1.0.0"": {
      ""type"": ""project"",
      ""serviceable"": false,
      ""sha512"": """"
    },
    ""Newtonsoft.Json/13.0.1"": {
      ""type"": ""package"",
      ""serviceable"": true,
      ""sha512"": ""sha512-ppPFpBcvxdsfUonNcvITKqLl3bqxWbDCZIzDWHzjpdAHRFfZe0Dw9HmA0+za13IdyrgJwpkDTDA9fHaxOrt20A=="",
      ""path"": ""newtonsoft.json/13.0.1"",
      ""hashPath"": ""newtonsoft.json.13.0.1.nupkg.sha512""
    }
  }
}
"u8;
    
    // This is a synthesised file which removes the bits we don't care about
    static ReadOnlySpan<byte> MultiDepsJson => @"
{
  ""targets"": {
    "".NETCoreApp,Version=v6.0"": {
      ""ScratchConsoleApp/1.0.0"": {
        ""dependencies"": {
          ""AsyncFixer"": ""1.6.0"",
          ""Octopus.Core"": ""1.0.0"",
          ""Octopus.Migrator.Core"": ""1.0.0"",
          ""Octopus.Nevermore.Analyzers"": ""21.0.1644"",
          ""Octopus.RoslynAnalyzers"": ""1.3.880"",
          ""Octopus.Server.Extensibility.Web"": ""1.0.0"",
          ""Octopus.Shared"": ""1.0.0"",
          ""Octopus.UnwantedMethodCallsAnalyzer"": ""0.2.440"",
          ""Serilog.Extensions.Logging"": ""3.1.0"",
          ""StyleCop.Analyzers"": ""1.0.2"",
          ""step-validation"": ""2.0.2""
        },
      },
      ""Octopus.Server.Extensibility.Authentication.OpenIDConnect.Common/1.0.0"": {
        ""dependencies"": {
          ""Microsoft.CSharp"": ""4.7.0"",
          ""Octopus.Configuration"": ""1.0.0"",
          ""Octopus.Data"": ""1.0.0"",
          ""Octopus.Diagnostics"": ""1.0.0"",
          ""Octopus.Server.Extensibility"": ""1.0.0"",
          ""Octopus.Server.Extensibility.Authentication"": ""1.0.0"",
          ""Octopus.Server.Extensibility.Web"": ""1.0.0"",
          ""Octopus.Time"": ""1.0.0"",
          ""System.IdentityModel.Tokens.Jwt"": ""6.27.0""
        }
      }
    }
  }
}
"u8;
}