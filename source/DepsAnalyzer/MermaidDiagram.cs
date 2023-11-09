using System.Text;
using DepsAnalyzer.Model;

namespace DepsAnalyzer;

public class MermaidDiagram
{
    public static string Generate(DependencyNode node, bool omitRootNode = false)
    {
        var sb = new StringBuilder();
        sb.AppendLine("flowchart TD");

        // nodes to Omit still get processed, but not printed
        var nodesToOmit = new HashSet<PackageAndVersion>();
        if (omitRootNode) nodesToOmit.Add(node.Package);
        
        // phase 1: preamble with all the nice names
        HashSet<PackageAndVersion> seenBefore = new();
        PrintPreamble(node, sb, seenBefore, nodesToOmit);

        sb.AppendLine();
        PrintGraph(node, sb, nodesToOmit);

        return sb.ToString();
    }

    static void PrintPreamble(DependencyNode node, StringBuilder output, HashSet<PackageAndVersion> seenBefore, HashSet<PackageAndVersion> nodesToOmit)
    {
        if (seenBefore.Contains(node.Package)) return;
        seenBefore.Add(node.Package);

        if (!nodesToOmit.Contains(node.Package))
        {
            output.AppendLine($"{NodeIdentifier(node.Package)}[\"{node.Package.Package}@{node.Package.Version}\"]");
        }

        foreach (var (_, child) in node.Dependencies)
        {
            PrintPreamble(child, output, seenBefore, nodesToOmit);
        }
    }
    
    static void PrintGraph(DependencyNode node, StringBuilder output, HashSet<PackageAndVersion> nodesToOmit)
    {
        foreach (var (_, child) in node.Dependencies)
        {
            if(nodesToOmit.Contains(node.Package)) continue;
            
            output.AppendLine($"{NodeIdentifier(node.Package)} --> {NodeIdentifier(child.Package)}");
        }
        
        foreach (var (_, child) in node.Dependencies)
        {
            PrintGraph(child, output, nodesToOmit);
        }
    }
    
    // mermaid node identifiers can't contain special characters
    static string NodeIdentifier(PackageAndVersion pv) => $"{pv.Package}_{pv.Version.Replace('.', '_')}";
}