using DepsAnalyzer;
using DepsAnalyzer.Model;

var depsFile = args.FirstOrDefault() ?? throw new ArgumentException("First command line arg must be a path to a deps file");

var content = File.ReadAllBytes(depsFile); // must be utf8

var parsed = DepsJsonParser.Parse(content);

DepsJsonConsolidator.Consolidate(parsed, ConsolidateBy.PrefixOnly, new List<(string Prefix, string ConsolidateInto)>
{
    //("System.", "System*"),
    ("Octopus.Server.Extensibility", "OctopusServerExtensibility*"),
    ("Octopus.Extensibility", "OctopusExtensibility*"),
    //("Octopus.", "OctopusOther*"), // order matters so we will pick out extensibility first
    ("Humanizer.", "Humanizer*"),
    ("Microsoft.EntityFrameworkCore", "EntityFrameworkCore*"),
    ("Microsoft.Extensions", "MicrosoftExtensions*"),
    ("Microsoft.Kiota", "MicrosoftKiota*"),
    ("Microsoft.IdentityModel", "MicrosoftIdentityModel*"),
    ("Microsoft.AspNetCore", "AspNetCore*"),
    ("MiniProfiler.AspNetCore", "MiniProfiler.AspNetCore*"),
    //("Nito.AsyncEx", "Nito.AsyncEx*"),
    ("Microsoft.CodeAnalysis", "Microsoft_CodeAnalysis*"),
    ("SumoLogic.Logging", "SumoLogic.Logging*"),
    ("Polly", "Polly*"),
    ("runtime.", "runtime*"),
    //("NuGet.", "NuGet*"),
    ("Swashbuckle.", "Swashbuckle*"),
});

var graph = DependencyGraph.Build(parsed!.Targets.FirstOrDefault().Value);

// if we want to generate a graph for just one thing e.g. MVC, we find that, then generate the graph from there
var filteredRoot = DependencyGraph.Filter(graph, "Octopus.Nevermore");

var mermaid = MermaidDiagram.Generate(filteredRoot, omitRootNode: true);

Console.WriteLine(mermaid);
