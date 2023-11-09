using DepsAnalyzer;

var depsFile = args.FirstOrDefault() ?? throw new ArgumentException("First command line arg must be a path to a deps file");

var content = File.ReadAllBytes(depsFile); // must be utf8

var parsed = DepsJsonParser.Parse(content);

var graph = DependencyGraph.Build(parsed!.Targets.FirstOrDefault().Value);

var mermaid = MermaidDiagram.Generate(graph, omitRootNode: true);

Console.WriteLine(mermaid);
